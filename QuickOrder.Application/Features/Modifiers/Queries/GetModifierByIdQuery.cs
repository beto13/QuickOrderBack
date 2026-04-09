using MediatR;
using QuickOrder.Application.Common;
using QuickOrder.Application.DTOs;
using QuickOrder.Application.Interfaces;

namespace QuickOrder.Application.Features.Modifiers.Queries;

public record GetModifierByIdQuery(int Id) : IRequest<Result<ModifierDto>>;

public class GetModifierByIdQueryHandler(IModifierRepository modifierRepository) : IRequestHandler<GetModifierByIdQuery, Result<ModifierDto>>
{
    public async Task<Result<ModifierDto>> Handle(GetModifierByIdQuery request, CancellationToken cancellationToken)
    {
        var modifier = await modifierRepository.FindByIdAsync(request.Id, cancellationToken);
        if (modifier is null)
            return Result<ModifierDto>.Fail(Error.NotFound($"Modificador {request.Id} no encontrado."));

        return Result<ModifierDto>.Ok(new ModifierDto(modifier.Id, modifier.ModifierGroupId, modifier.Name, modifier.Description));
    }
}
