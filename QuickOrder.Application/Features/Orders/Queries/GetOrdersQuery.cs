using MediatR;
using QuickOrder.Application.DTOs;
using QuickOrder.Application.Interfaces;

namespace QuickOrder.Application.Features.Orders.Queries;

public record GetActiveOrdersQuery : IRequest<List<OrderDto>>;

public class GetActiveOrdersQueryHandler(IOrderRepository orderRepository) : IRequestHandler<GetActiveOrdersQuery, List<OrderDto>>
{
    public async Task<List<OrderDto>> Handle(GetActiveOrdersQuery request, CancellationToken cancellationToken)
    {
        var orders = await orderRepository.GetActiveAsync(cancellationToken);

        return orders.Select(o => new OrderDto(
            o.Id,
            o.TableId,
            o.Table.Number,
            o.MenuId,
            o.Menu.Name,
            o.Status.ToString(),
            o.Notes,
            o.CreatedAt,
            o.Items.Select(i => new OrderItemDto(i.MenuProductId, i.MenuProduct.Product.Name, i.Quantity, i.UnitPrice, i.Notes)).ToList()
        )).ToList();
    }
}
