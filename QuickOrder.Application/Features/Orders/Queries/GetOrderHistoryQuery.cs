using MediatR;
using QuickOrder.Application.Common;
using QuickOrder.Application.DTOs;
using QuickOrder.Application.Interfaces;

namespace QuickOrder.Application.Features.Orders.Queries;

public record GetOrderHistoryQuery(int PageNumber = 1, int PageSize = 20) : IRequest<PaginatedResponse<OrderHistoryDto>>;

public class GetOrderHistoryQueryHandler(IOrderRepository orderRepository) : IRequestHandler<GetOrderHistoryQuery, PaginatedResponse<OrderHistoryDto>>
{
    public async Task<PaginatedResponse<OrderHistoryDto>> Handle(GetOrderHistoryQuery request, CancellationToken cancellationToken)
    {
        var (orders, total) = await orderRepository.GetPagedAsync(request.PageNumber, request.PageSize, cancellationToken);

        var dtos = orders.Select(o => new OrderHistoryDto(
            o.Id,
            o.TableId,
            o.Table.Number,
            o.MenuId,
            o.Menu.Name,
            o.Status.ToString(),
            o.Notes,
            o.CreatedAt,
            o.UpdatedAt,
            o.Items.Select(i => new OrderItemDto(i.MenuProductId, i.MenuProduct.Product.Name, i.Quantity, i.UnitPrice, i.Notes)).ToList(),
            o.StatusHistory.OrderBy(h => h.ChangedAt)
                .Select(h => new StatusMovementDto(h.FromStatus, h.ToStatus, h.ChangedAt)).ToList()
        )).ToList();

        return PaginatedResponse<OrderHistoryDto>.Create(dtos, total, request.PageNumber, request.PageSize);
    }
}
