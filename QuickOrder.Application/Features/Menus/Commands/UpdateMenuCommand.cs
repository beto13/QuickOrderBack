using MediatR;
using QuickOrder.Application.DTOs;
using QuickOrder.Application.Interfaces;

namespace QuickOrder.Application.Features.Menus.Commands;

public record UpdateMenuCommand(int Id, string? Name, bool? IsActive) : IRequest<MenuDto>;

public class UpdateMenuCommandHandler(
    IMenuRepository menuRepository,
    IUnitOfWork unitOfWork) : IRequestHandler<UpdateMenuCommand, MenuDto>
{
    public async Task<MenuDto> Handle(UpdateMenuCommand request, CancellationToken cancellationToken)
    {
        var menu = await menuRepository.FindByIdAsync(request.Id, cancellationToken)
            ?? throw new KeyNotFoundException($"Menú {request.Id} no encontrado.");

        if (request.Name is not null) menu.Name = request.Name;
        if (request.IsActive.HasValue) menu.IsActive = request.IsActive.Value;

        await unitOfWork.SaveChangesAsync(cancellationToken);

        return new MenuDto(menu.Id, menu.Name, menu.IsActive);
    }
}
