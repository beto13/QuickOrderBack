using MediatR;
using QuickOrder.Application.Common;
using QuickOrder.Application.DTOs;
using QuickOrder.Application.Interfaces;

namespace QuickOrder.Application.Features.Menu.Queries;

public record GetMenuQuery(int MenuId) : IRequest<Result<List<MenuCategoryDto>>>;

public record MenuItemDto(int Id, string Name, string? Description, string? ImageUrl, decimal Price, int CategoryId, string CategoryName, bool HasModifierGroups);
public record MenuCategoryDto(int Id, string Name, int DisplayOrder, List<MenuItemDto> Products);

public class GetMenuQueryHandler(ICategoryRepository categoryRepository) : IRequestHandler<GetMenuQuery, Result<List<MenuCategoryDto>>>
{
    public async Task<Result<List<MenuCategoryDto>>> Handle(GetMenuQuery request, CancellationToken cancellationToken)
    {
        var categories = await categoryRepository.GetAllWithMenuProductsAsync(request.MenuId, cancellationToken);

        var result = categories.Select(c => new MenuCategoryDto(
            c.Id,
            c.Name,
            c.DisplayOrder,
            c.MenuProducts
                .Where(mp => mp.IsAvailable)
                .Select(mp => new MenuItemDto(
                    mp.Id,
                    mp.Product.Name,
                    mp.Product.Description,
                    mp.Product.ImageUrl,
                    mp.Price,
                    c.Id,
                    c.Name,
                    mp.MenuModifiers.Any(mm => mm.IsAvailable)))
                .ToList()
        )).ToList();

        return Result<List<MenuCategoryDto>>.Ok(result);
    }
}
