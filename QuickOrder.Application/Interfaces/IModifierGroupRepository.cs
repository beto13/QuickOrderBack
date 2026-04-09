using QuickOrder.Domain.Entities;

namespace QuickOrder.Application.Interfaces;

public interface IModifierGroupRepository
{
    Task<(List<ModifierGroup> Items, int TotalCount)> GetPagedAsync(int pageNumber, int pageSize, CancellationToken cancellationToken = default);
    Task<ModifierGroup?> FindByIdAsync(int id, CancellationToken cancellationToken = default);
    Task<List<(ModifierGroup Group, List<(Modifier Modifier, decimal ExtraPrice)> Items)>> GetForMenuProductWithPricesAsync(int menuProductId, CancellationToken cancellationToken = default);
    void Add(ModifierGroup group);
    void Remove(ModifierGroup group);
}
