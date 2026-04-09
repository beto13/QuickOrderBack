using MediatR;
using QuickOrder.Application.Common;
using QuickOrder.Application.Interfaces;

namespace QuickOrder.Application.Features.Menu.Queries;

public record ProductModifierOptionDto(int Id, string Name, decimal ExtraPrice);
public record ProductModifierGroupDto(int Id, string Name, bool IsRequired, int MinSelections, int MaxSelections, List<ProductModifierOptionDto> Options);

public record GetProductModifiersQuery(int MenuProductId) : IRequest<Result<List<ProductModifierGroupDto>>>;

public class GetProductModifiersQueryHandler(IModifierGroupRepository modifierGroupRepository)
    : IRequestHandler<GetProductModifiersQuery, Result<List<ProductModifierGroupDto>>>
{
    public async Task<Result<List<ProductModifierGroupDto>>> Handle(GetProductModifiersQuery request, CancellationToken cancellationToken)
    {
        var groups = await modifierGroupRepository.GetForMenuProductWithPricesAsync(request.MenuProductId, cancellationToken);

        var result = groups.Select(g => new ProductModifierGroupDto(
            g.Group.Id,
            g.Group.Name,
            g.Group.IsRequired,
            g.Group.MinSelections,
            g.Group.MaxSelections,
            g.Items.Select(i => new ProductModifierOptionDto(i.Modifier.Id, i.Modifier.Name, i.ExtraPrice)).ToList()
        )).ToList();

        return Result<List<ProductModifierGroupDto>>.Ok(result);
    }
}
