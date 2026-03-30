using MediatR;
using QuickOrder.Application.DTOs;
using QuickOrder.Application.Interfaces;

namespace QuickOrder.Application.Features.Menus.Queries;

public record GetMenuByIdQuery(int Id) : IRequest<MenuDto>;

public class GetMenuByIdQueryHandler(IMenuRepository menuRepository) : IRequestHandler<GetMenuByIdQuery, MenuDto>
{
    public async Task<MenuDto> Handle(GetMenuByIdQuery request, CancellationToken cancellationToken)
    {
        var menu = await menuRepository.FindByIdAsync(request.Id, cancellationToken)
            ?? throw new KeyNotFoundException($"Menú {request.Id} no encontrado.");

        return new MenuDto(menu.Id, menu.Name, menu.IsActive);
    }
}
