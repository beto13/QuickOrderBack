using MediatR;
using QuickOrder.Application.Common;
using QuickOrder.Application.Interfaces;

namespace QuickOrder.Application.Features.ModifierGroups.Commands;

public record DeleteModifierGroupCommand(int Id) : IRequest<Result>;

public class DeleteModifierGroupCommandHandler(
    IModifierGroupRepository modifierGroupRepository,
    IUnitOfWork unitOfWork) : IRequestHandler<DeleteModifierGroupCommand, Result>
{
    public async Task<Result> Handle(DeleteModifierGroupCommand request, CancellationToken cancellationToken)
    {
        var group = await modifierGroupRepository.FindByIdAsync(request.Id, cancellationToken);
        if (group is null)
            return Result.Fail(Error.NotFound($"Grupo de modificadores {request.Id} no encontrado."));

        modifierGroupRepository.Remove(group);
        await unitOfWork.SaveChangesAsync(cancellationToken);

        return Result.Ok();
    }
}
