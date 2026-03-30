using MediatR;
using QuickOrder.Application.Interfaces;

namespace QuickOrder.Application.Features.ModifierGroups.Commands;

public record DeleteModifierGroupCommand(int Id) : IRequest;

public class DeleteModifierGroupCommandHandler(
    IModifierGroupRepository modifierGroupRepository,
    IUnitOfWork unitOfWork) : IRequestHandler<DeleteModifierGroupCommand>
{
    public async Task Handle(DeleteModifierGroupCommand request, CancellationToken cancellationToken)
    {
        var group = await modifierGroupRepository.FindByIdAsync(request.Id, cancellationToken)
            ?? throw new KeyNotFoundException($"Grupo de modificadores {request.Id} no encontrado.");

        modifierGroupRepository.Remove(group);
        await unitOfWork.SaveChangesAsync(cancellationToken);
    }
}
