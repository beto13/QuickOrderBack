using MediatR;
using QuickOrder.Application.Common;
using QuickOrder.Application.DTOs;
using QuickOrder.Application.Interfaces;

namespace QuickOrder.Application.Features.Modifiers.Queries;

public record GetModifiersByGroupQuery(int ModifierGroupId, int PageNumber = 1, int PageSize = 20) : IRequest<PaginatedResponse<ModifierDto>>;

public class GetModifiersByGroupQueryHandler(IModifierRepository modifierRepository) : IRequestHandler<GetModifiersByGroupQuery, PaginatedResponse<ModifierDto>>
{
    public async Task<PaginatedResponse<ModifierDto>> Handle(GetModifiersByGroupQuery request, CancellationToken cancellationToken)
    {
        var (items, total) = await modifierRepository.GetPagedAsync(request.ModifierGroupId, request.PageNumber, request.PageSize, cancellationToken);

        var dtos = items.Select(m => new ModifierDto(m.Id, m.ModifierGroupId, m.Name, m.Description)).ToList();

        return PaginatedResponse<ModifierDto>.Create(dtos, total, request.PageNumber, request.PageSize);
    }
}
