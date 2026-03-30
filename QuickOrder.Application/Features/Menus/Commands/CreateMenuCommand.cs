using MediatR;
using QuickOrder.Application.DTOs;
using QuickOrder.Application.Interfaces;
using QuickOrder.Domain.Entities;

namespace QuickOrder.Application.Features.Menus.Commands;

public record CreateMenuCommand(string Name) : IRequest<MenuDto>;

public class CreateMenuCommandHandler(
    IMenuRepository menuRepository,
    IUnitOfWork unitOfWork) : IRequestHandler<CreateMenuCommand, MenuDto>
{
    public async Task<MenuDto> Handle(CreateMenuCommand request, CancellationToken cancellationToken)
    {
        var menu = new Menu { Name = request.Name };

        menuRepository.Add(menu);
        await unitOfWork.SaveChangesAsync(cancellationToken);

        return new MenuDto(menu.Id, menu.Name, menu.IsActive);
    }
}
