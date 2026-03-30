using MediatR;
using QuickOrder.Application.Interfaces;

namespace QuickOrder.Application.Features.Modifiers.Commands;

public record DeleteModifierCommand(int Id) : IRequest;

public class DeleteModifierCommandHandler(
    IModifierRepository modifierRepository,
    IUnitOfWork unitOfWork) : IRequestHandler<DeleteModifierCommand>
{
    public async Task Handle(DeleteModifierCommand request, CancellationToken cancellationToken)
    {
        var modifier = await modifierRepository.FindByIdAsync(request.Id, cancellationToken)
            ?? throw new KeyNotFoundException($"Modificador {request.Id} no encontrado.");

        modifierRepository.Remove(modifier);
        await unitOfWork.SaveChangesAsync(cancellationToken);
    }
}
