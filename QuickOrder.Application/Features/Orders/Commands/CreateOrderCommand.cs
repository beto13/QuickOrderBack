using MediatR;
using QuickOrder.Application.DTOs;
using QuickOrder.Application.Interfaces;
using QuickOrder.Application.Messages;
using QuickOrder.Domain.Entities;

namespace QuickOrder.Application.Features.Orders.Commands;

public record CreateOrderCommand(int TableId, int MenuId, string? Notes, List<CreateOrderItemRequest> Items) : IRequest<OrderDto>;

public class CreateOrderCommandHandler(
    ITableRepository tableRepository,
    IMenuProductRepository menuProductRepository,
    IOrderRepository orderRepository,
    IUnitOfWork unitOfWork,
    IOrderHub hub,
    IMessagePublisher publisher) : IRequestHandler<CreateOrderCommand, OrderDto>
{
    public async Task<OrderDto> Handle(CreateOrderCommand request, CancellationToken cancellationToken)
    {
        var table = await tableRepository.FindByIdAsync(request.TableId, cancellationToken)
            ?? throw new KeyNotFoundException($"Mesa {request.TableId} no encontrada.");

        var menuProducts = await menuProductRepository.GetByIdsAsync(
            request.Items.Select(i => i.MenuProductId), cancellationToken);

        var orderItems = new List<OrderItem>();
        foreach (var item in request.Items)
        {
            if (!menuProducts.TryGetValue(item.MenuProductId, out var menuProduct))
                throw new KeyNotFoundException($"Producto de menú {item.MenuProductId} no disponible.");

            orderItems.Add(new OrderItem
            {
                MenuProductId = item.MenuProductId,
                Quantity = item.Quantity,
                UnitPrice = menuProduct.Price,
                Notes = item.Notes
            });
        }

        var order = new Order
        {
            TableId = request.TableId,
            MenuId = request.MenuId,
            Notes = request.Notes,
            Items = orderItems
        };

        orderRepository.Add(order);
        await unitOfWork.SaveChangesAsync(cancellationToken);

        var menuName = menuProducts.Values.FirstOrDefault()?.Menu?.Name ?? string.Empty;

        var dto = new OrderDto(
            order.Id,
            table.Id,
            table.Number,
            request.MenuId,
            menuName,
            order.Status.ToString(),
            order.Notes,
            order.CreatedAt,
            order.Items.Select(i => new OrderItemDto(
                i.MenuProductId,
                menuProducts[i.MenuProductId].Product.Name,
                i.Quantity,
                i.UnitPrice,
                i.Notes
            )).ToList()
        );

        await hub.NotifyNewOrder(dto);

        await publisher.PublishAsync(new OrderCreatedEvent(
            order.Id,
            table.Id,
            table.Number,
            order.Notes,
            order.CreatedAt,
            order.Items.Select(i => new OrderCreatedItemEvent(
                i.MenuProductId,
                menuProducts[i.MenuProductId].Product.Name,
                i.Quantity,
                i.UnitPrice,
                i.Notes
            )).ToList()
        ), cancellationToken);

        return dto;
    }
}
