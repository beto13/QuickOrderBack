using MediatR;
using QuickOrder.Application.DTOs;
using QuickOrder.Application.Interfaces;

namespace QuickOrder.Application.Features.Modifiers.Commands;

public record UpdateModifierCommand(int Id, string? Name, string? Description) : IRequest<ModifierDto>;

public class UpdateModifierCommandHandler(
    IModifierRepository modifierRepository,
    IUnitOfWork unitOfWork) : IRequestHandler<UpdateModifierCommand, ModifierDto>
{
    public async Task<ModifierDto> Handle(UpdateModifierCommand request, CancellationToken cancellationToken)
    {
        var modifier = await modifierRepository.FindByIdAsync(request.Id, cancellationToken)
            ?? throw new KeyNotFoundException($"Modificador {request.Id} no encontrado.");

        if (request.Name is not null) modifier.Name = request.Name;
        if (request.Description is not null) modifier.Description = request.Description;

        await unitOfWork.SaveChangesAsync(cancellationToken);

        return new ModifierDto(modifier.Id, modifier.ModifierGroupId, modifier.Name, modifier.Description);
    }
}
