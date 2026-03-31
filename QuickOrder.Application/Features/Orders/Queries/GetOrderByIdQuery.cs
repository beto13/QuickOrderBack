using MediatR;
using QuickOrder.Application.DTOs;
using QuickOrder.Application.Interfaces;

namespace QuickOrder.Application.Features.Orders.Queries;

public record GetOrderByIdQuery(int Id) : IRequest<OrderDto>;

public class GetOrderByIdQueryHandler(IOrderRepository orderRepository) : IRequestHandler<GetOrderByIdQuery, OrderDto>
{
    public async Task<OrderDto> Handle(GetOrderByIdQuery request, CancellationToken cancellationToken)
    {
        var order = await orderRepository.GetByIdWithDetailsAsync(request.Id, cancellationToken)
            ?? throw new KeyNotFoundException($"Pedido {request.Id} no encontrado.");

        return new OrderDto(
            order.Id,
            order.TableId,
            order.Table.Number,
            order.MenuId,
            order.Menu.Name,
            order.Status.ToString(),
            order.Notes,
            order.CreatedAt,
            order.Items.Select(i => new OrderItemDto(i.MenuProductId, i.MenuProduct.Product.Name, i.Quantity, i.UnitPrice, i.Notes)).ToList()
        );
    }
}
