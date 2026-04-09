using MediatR;
using QuickOrder.Application.Common;
using QuickOrder.Application.DTOs;
using QuickOrder.Application.Interfaces;

namespace QuickOrder.Application.Features.Tables.Queries;

public record GetAllTablesQuery(int PageNumber = 1, int PageSize = 20) : IRequest<Result<PaginatedResponse<TableDto>>>;

public class GetAllTablesQueryHandler(ITableRepository tableRepository) : IRequestHandler<GetAllTablesQuery, Result<PaginatedResponse<TableDto>>>
{
    public async Task<Result<PaginatedResponse<TableDto>>> Handle(GetAllTablesQuery request, CancellationToken cancellationToken)
    {
        var (items, total) = await tableRepository.GetPagedAsync(request.PageNumber, request.PageSize, cancellationToken);

        var dtos = items.Select(t => new TableDto(t.Id, t.Number, t.IsActive, t.MenuId)).ToList();

        return Result<PaginatedResponse<TableDto>>.Ok(
            PaginatedResponse<TableDto>.Create(dtos, total, request.PageNumber, request.PageSize));
    }
}
