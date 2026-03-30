using MediatR;
using QuickOrder.Application.DTOs;
using QuickOrder.Application.Interfaces;

namespace QuickOrder.Application.Features.Modifiers.Queries;

public record GetModifierByIdQuery(int Id) : IRequest<ModifierDto>;

public class GetModifierByIdQueryHandler(IModifierRepository modifierRepository) : IRequestHandler<GetModifierByIdQuery, ModifierDto>
{
    public async Task<ModifierDto> Handle(GetModifierByIdQuery request, CancellationToken cancellationToken)
    {
        var modifier = await modifierRepository.FindByIdAsync(request.Id, cancellationToken)
            ?? throw new KeyNotFoundException($"Modificador {request.Id} no encontrado.");

        return new ModifierDto(modifier.Id, modifier.ModifierGroupId, modifier.Name, modifier.Description);
    }
}
