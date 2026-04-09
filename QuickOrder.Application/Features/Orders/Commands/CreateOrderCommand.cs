using MediatR;
using QuickOrder.Application.Common;
using QuickOrder.Application.DTOs;
using QuickOrder.Application.Interfaces;
using QuickOrder.Application.Messages;
using QuickOrder.Domain.Entities;

namespace QuickOrder.Application.Features.Orders.Commands;

public record CreateOrderCommand(int TableId, int MenuId, string? Notes, List<CreateOrderItemRequest> Items) : IRequest<Result<OrderDto>>;

public class CreateOrderCommandHandler(
    ITableRepository tableRepository,
    IMenuProductRepository menuProductRepository,
    IOrderRepository orderRepository,
    IUnitOfWork unitOfWork,
    IOrderHub hub,
    IMessagePublisher publisher) : IRequestHandler<CreateOrderCommand, Result<OrderDto>>
{
    public async Task<Result<OrderDto>> Handle(CreateOrderCommand request, CancellationToken cancellationToken)
    {
        var table = await tableRepository.FindByIdAsync(request.TableId, cancellationToken);
        if (table is null)
            return Result<OrderDto>.Fail(Error.NotFound($"Mesa {request.TableId} no encontrada."));

        var menuProducts = await menuProductRepository.GetByIdsAsync(
            request.Items.Select(i => i.MenuProductId), cancellationToken);

        var order = new Order
        {
            TableId = request.TableId,
            MenuId = request.MenuId,
            Notes = request.Notes,
            Items = request.Items.Select(i => new OrderItem
            {
                MenuProductId = i.MenuProductId,
                Quantity = i.Quantity,
                UnitPrice = menuProducts[i.MenuProductId].Price,
                Notes = i.Notes
            }).ToList()
        };

        orderRepository.Add(order);
        await unitOfWork.SaveChangesAsync(cancellationToken);

        var dto = new OrderDto(
            order.Id,
            table.Id,
            table.Number,
            request.MenuId,
            menuProducts.Values.First().Menu?.Name ?? string.Empty,
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

        return Result<OrderDto>.Ok(dto);
    }
}
