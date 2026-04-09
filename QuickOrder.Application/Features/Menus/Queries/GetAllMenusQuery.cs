using MediatR;
using QuickOrder.Application.Common;
using QuickOrder.Application.DTOs;
using QuickOrder.Application.Interfaces;

namespace QuickOrder.Application.Features.Menus.Queries;

public record GetAllMenusQuery(int PageNumber = 1, int PageSize = 20) : IRequest<Result<PaginatedResponse<MenuDto>>>;

public class GetAllMenusQueryHandler(IMenuRepository menuRepository) : IRequestHandler<GetAllMenusQuery, Result<PaginatedResponse<MenuDto>>>
{
    public async Task<Result<PaginatedResponse<MenuDto>>> Handle(GetAllMenusQuery request, CancellationToken cancellationToken)
    {
        var (items, total) = await menuRepository.GetPagedAsync(request.PageNumber, request.PageSize, cancellationToken);

        var dtos = items.Select(m => new MenuDto(m.Id, m.Name, m.IsActive)).ToList();

        return Result<PaginatedResponse<MenuDto>>.Ok(
            PaginatedResponse<MenuDto>.Create(dtos, total, request.PageNumber, request.PageSize));
    }
}
