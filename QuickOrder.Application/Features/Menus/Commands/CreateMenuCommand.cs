using MediatR;
using QuickOrder.Application.Common;
using QuickOrder.Application.DTOs;
using QuickOrder.Application.Interfaces;
using MenuEntity = QuickOrder.Domain.Entities.Menu;

namespace QuickOrder.Application.Features.Menus.Commands;

public record CreateMenuCommand(string Name) : IRequest<Result<MenuDto>>;

public class CreateMenuCommandHandler(
    IMenuRepository menuRepository,
    IUnitOfWork unitOfWork) : IRequestHandler<CreateMenuCommand, Result<MenuDto>>
{
    public async Task<Result<MenuDto>> Handle(CreateMenuCommand request, CancellationToken cancellationToken)
    {
        var menu = new MenuEntity { Name = request.Name };

        menuRepository.Add(menu);
        await unitOfWork.SaveChangesAsync(cancellationToken);

        return Result<MenuDto>.Ok(new MenuDto(menu.Id, menu.Name, menu.IsActive));
    }
}
