using MediatR;
using QuickOrder.Application.Common;
using QuickOrder.Application.DTOs;
using QuickOrder.Application.Interfaces;

namespace QuickOrder.Application.Features.Menus.Commands;

public record UpdateMenuCommand(int Id, string? Name, bool? IsActive) : IRequest<Result<MenuDto>>;

public class UpdateMenuCommandHandler(
    IMenuRepository menuRepository,
    IUnitOfWork unitOfWork) : IRequestHandler<UpdateMenuCommand, Result<MenuDto>>
{
    public async Task<Result<MenuDto>> Handle(UpdateMenuCommand request, CancellationToken cancellationToken)
    {
        var menu = await menuRepository.FindByIdAsync(request.Id, cancellationToken);
        if (menu is null)
            return Result<MenuDto>.Fail(Error.NotFound($"Menú {request.Id} no encontrado."));

        if (request.Name is not null) menu.Name = request.Name;
        if (request.IsActive.HasValue) menu.IsActive = request.IsActive.Value;

        await unitOfWork.SaveChangesAsync(cancellationToken);

        return Result<MenuDto>.Ok(new MenuDto(menu.Id, menu.Name, menu.IsActive));
    }
}
