using MediatR;
using QuickOrder.Application.DTOs;
using QuickOrder.Application.Interfaces;

namespace QuickOrder.Application.Features.ModifierGroups.Queries;

public record GetModifierGroupByIdQuery(int Id) : IRequest<ModifierGroupDto>;

public class GetModifierGroupByIdQueryHandler(IModifierGroupRepository modifierGroupRepository) : IRequestHandler<GetModifierGroupByIdQuery, ModifierGroupDto>
{
    public async Task<ModifierGroupDto> Handle(GetModifierGroupByIdQuery request, CancellationToken cancellationToken)
    {
        var group = await modifierGroupRepository.FindByIdAsync(request.Id, cancellationToken)
            ?? throw new KeyNotFoundException($"Grupo de modificadores {request.Id} no encontrado.");

        return new ModifierGroupDto(group.Id, group.Name, group.MinSelections, group.MaxSelections, group.IsRequired, group.ProductId, group.CategoryId);
    }
}
