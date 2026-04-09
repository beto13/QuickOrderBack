using MediatR;
using QuickOrder.Application.Common;
using QuickOrder.Application.Interfaces;

namespace QuickOrder.Application.Features.Modifiers.Commands;

public record DeleteModifierCommand(int Id) : IRequest<Result>;

public class DeleteModifierCommandHandler(
    IModifierRepository modifierRepository,
    IUnitOfWork unitOfWork) : IRequestHandler<DeleteModifierCommand, Result>
{
    public async Task<Result> Handle(DeleteModifierCommand request, CancellationToken cancellationToken)
    {
        var modifier = await modifierRepository.FindByIdAsync(request.Id, cancellationToken);
        if (modifier is null)
            return Result.Fail(Error.NotFound($"Modificador {request.Id} no encontrado."));

        modifierRepository.Remove(modifier);
        await unitOfWork.SaveChangesAsync(cancellationToken);

        return Result.Ok();
    }
}
