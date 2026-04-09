using MediatR;
using QuickOrder.Application.Common;
using QuickOrder.Application.DTOs;
using QuickOrder.Application.Interfaces;
using QuickOrder.Domain.Entities;

namespace QuickOrder.Application.Features.Modifiers.Commands;

public record CreateModifierCommand(int ModifierGroupId, string Name, string? Description) : IRequest<Result<ModifierDto>>;

public class CreateModifierCommandHandler(
    IModifierGroupRepository modifierGroupRepository,
    IModifierRepository modifierRepository,
    IUnitOfWork unitOfWork) : IRequestHandler<CreateModifierCommand, Result<ModifierDto>>
{
    public async Task<Result<ModifierDto>> Handle(CreateModifierCommand request, CancellationToken cancellationToken)
    {
        var group = await modifierGroupRepository.FindByIdAsync(request.ModifierGroupId, cancellationToken);
        if (group is null)
            return Result<ModifierDto>.Fail(Error.NotFound($"Grupo de modificadores {request.ModifierGroupId} no encontrado."));

        var modifier = new Modifier
        {
            ModifierGroupId = request.ModifierGroupId,
            Name = request.Name,
            Description = request.Description
        };

        modifierRepository.Add(modifier);
        await unitOfWork.SaveChangesAsync(cancellationToken);

        return Result<ModifierDto>.Ok(new ModifierDto(modifier.Id, modifier.ModifierGroupId, modifier.Name, modifier.Description));
    }
}
