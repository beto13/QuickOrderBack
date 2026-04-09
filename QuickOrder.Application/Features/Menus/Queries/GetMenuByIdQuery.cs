using MediatR;
using QuickOrder.Application.Common;
using QuickOrder.Application.DTOs;
using QuickOrder.Application.Interfaces;

namespace QuickOrder.Application.Features.Menus.Queries;

public record GetMenuByIdQuery(int Id) : IRequest<Result<MenuDto>>;

public class GetMenuByIdQueryHandler(IMenuRepository menuRepository) : IRequestHandler<GetMenuByIdQuery, Result<MenuDto>>
{
    public async Task<Result<MenuDto>> Handle(GetMenuByIdQuery request, CancellationToken cancellationToken)
    {
        var menu = await menuRepository.FindByIdAsync(request.Id, cancellationToken);
        if (menu is null)
            return Result<MenuDto>.Fail(Error.NotFound($"Menú {request.Id} no encontrado."));

        return Result<MenuDto>.Ok(new MenuDto(menu.Id, menu.Name, menu.IsActive));
    }
}
