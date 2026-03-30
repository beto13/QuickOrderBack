using MediatR;
using QuickOrder.Application.Common;
using QuickOrder.Application.DTOs;
using QuickOrder.Application.Interfaces;

namespace QuickOrder.Application.Features.ModifierGroups.Queries;

public record GetAllModifierGroupsQuery(int PageNumber = 1, int PageSize = 20) : IRequest<PaginatedResponse<ModifierGroupDto>>;

public class GetAllModifierGroupsQueryHandler(IModifierGroupRepository modifierGroupRepository) : IRequestHandler<GetAllModifierGroupsQuery, PaginatedResponse<ModifierGroupDto>>
{
    public async Task<PaginatedResponse<ModifierGroupDto>> Handle(GetAllModifierGroupsQuery request, CancellationToken cancellationToken)
    {
        var (items, total) = await modifierGroupRepository.GetPagedAsync(request.PageNumber, request.PageSize, cancellationToken);

        var dtos = items.Select(g => new ModifierGroupDto(g.Id, g.Name, g.MinSelections, g.MaxSelections, g.IsRequired, g.ProductId, g.CategoryId)).ToList();

        return PaginatedResponse<ModifierGroupDto>.Create(dtos, total, request.PageNumber, request.PageSize);
    }
}
