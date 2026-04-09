using MediatR;
using QuickOrder.Application.Common;
using QuickOrder.Application.DTOs;
using QuickOrder.Application.Interfaces;

namespace QuickOrder.Application.Features.Orders.Queries;

public record GetActiveOrdersQuery : IRequest<Result<List<OrderDto>>>;

public class GetActiveOrdersQueryHandler(IOrderRepository orderRepository) : IRequestHandler<GetActiveOrdersQuery, Result<List<OrderDto>>>
{
    public async Task<Result<List<OrderDto>>> Handle(GetActiveOrdersQuery request, CancellationToken cancellationToken)
    {
        var orders = await orderRepository.GetActiveAsync(cancellationToken);

        var dtos = orders.Select(o => new OrderDto(
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

        return Result<List<OrderDto>>.Ok(dtos);
    }
}
