using MediatR;
using QuickOrder.Application.DTOs;
using QuickOrder.Application.Interfaces;
using QuickOrder.Domain.Entities;

namespace QuickOrder.Application.Features.Modifiers.Commands;

public record CreateModifierCommand(int ModifierGroupId, string Name, string? Description) : IRequest<ModifierDto>;

public class CreateModifierCommandHandler(
    IModifierGroupRepository modifierGroupRepository,
    IModifierRepository modifierRepository,
    IUnitOfWork unitOfWork) : IRequestHandler<CreateModifierCommand, ModifierDto>
{
    public async Task<ModifierDto> Handle(CreateModifierCommand request, CancellationToken cancellationToken)
    {
        _ = await modifierGroupRepository.FindByIdAsync(request.ModifierGroupId, cancellationToken)
            ?? throw new KeyNotFoundException($"Grupo de modificadores {request.ModifierGroupId} no encontrado.");

        var modifier = new Modifier
        {
            ModifierGroupId = request.ModifierGroupId,
            Name = request.Name,
            Description = request.Description
        };

        modifierRepository.Add(modifier);
        await unitOfWork.SaveChangesAsync(cancellationToken);

        return new ModifierDto(modifier.Id, modifier.ModifierGroupId, modifier.Name, modifier.Description);
    }
}
