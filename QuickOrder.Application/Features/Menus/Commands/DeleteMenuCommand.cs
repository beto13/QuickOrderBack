using MediatR;
using QuickOrder.Application.Interfaces;

namespace QuickOrder.Application.Features.Menus.Commands;

public record DeleteMenuCommand(int Id) : IRequest;

public class DeleteMenuCommandHandler(
    IMenuRepository menuRepository,
    IUnitOfWork unitOfWork) : IRequestHandler<DeleteMenuCommand>
{
    public async Task Handle(DeleteMenuCommand request, CancellationToken cancellationToken)
    {
        var menu = await menuRepository.FindByIdAsync(request.Id, cancellationToken)
            ?? throw new KeyNotFoundException($"Menú {request.Id} no encontrado.");

        menu.IsActive = false;
        await unitOfWork.SaveChangesAsync(cancellationToken);
    }
}
