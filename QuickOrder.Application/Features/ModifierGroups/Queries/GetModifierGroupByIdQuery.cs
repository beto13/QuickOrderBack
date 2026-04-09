using MediatR;
using QuickOrder.Application.Common;
using QuickOrder.Application.DTOs;
using QuickOrder.Application.Interfaces;

namespace QuickOrder.Application.Features.ModifierGroups.Queries;

public record GetModifierGroupByIdQuery(int Id) : IRequest<Result<ModifierGroupDto>>;

public class GetModifierGroupByIdQueryHandler(IModifierGroupRepository modifierGroupRepository) : IRequestHandler<GetModifierGroupByIdQuery, Result<ModifierGroupDto>>
{
    public async Task<Result<ModifierGroupDto>> Handle(GetModifierGroupByIdQuery request, CancellationToken cancellationToken)
    {
        var group = await modifierGroupRepository.FindByIdAsync(request.Id, cancellationToken);
        if (group is null)
            return Result<ModifierGroupDto>.Fail(Error.NotFound($"Grupo de modificadores {request.Id} no encontrado."));

        return Result<ModifierGroupDto>.Ok(new ModifierGroupDto(group.Id, group.Name, group.MinSelections, group.MaxSelections, group.IsRequired, group.ProductId, group.CategoryId));
    }
}
