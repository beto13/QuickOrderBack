using MediatR;
using QuickOrder.Application.Common;
using QuickOrder.Application.Interfaces;

namespace QuickOrder.Application.Features.Menus.Commands;

public record DeleteMenuCommand(int Id) : IRequest<Result>;

public class DeleteMenuCommandHandler(
    IMenuRepository menuRepository,
    IUnitOfWork unitOfWork) : IRequestHandler<DeleteMenuCommand, Result>
{
    public async Task<Result> Handle(DeleteMenuCommand request, CancellationToken cancellationToken)
    {
        var menu = await menuRepository.FindByIdAsync(request.Id, cancellationToken);
        if (menu is null)
            return Result.Fail(Error.NotFound($"Menú {request.Id} no encontrado."));

        menu.IsActive = false;
        await unitOfWork.SaveChangesAsync(cancellationToken);

        return Result.Ok();
    }
}
