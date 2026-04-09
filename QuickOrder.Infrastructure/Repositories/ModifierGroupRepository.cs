using Microsoft.EntityFrameworkCore;
using QuickOrder.Application.Interfaces;
using QuickOrder.Domain.Entities;
using QuickOrder.Infrastructure.Persistence;

namespace QuickOrder.Infrastructure.Repositories;

public class ModifierGroupRepository(AppDbContext db) : IModifierGroupRepository
{
    public async Task<(List<ModifierGroup> Items, int TotalCount)> GetPagedAsync(int pageNumber, int pageSize, CancellationToken cancellationToken = default)
    {
        var query = db.ModifierGroups.AsNoTracking().OrderBy(g => g.Name);
        var total = await query.CountAsync(cancellationToken);
        var items = await query
            .Skip((pageNumber - 1) * pageSize)
            .Take(pageSize)
            .ToListAsync(cancellationToken);
        return (items, total);
    }

    public Task<ModifierGroup?> FindByIdAsync(int id, CancellationToken cancellationToken = default) =>
        db.ModifierGroups.FindAsync([id], cancellationToken).AsTask();

    public async Task<List<(ModifierGroup Group, List<(Modifier Modifier, decimal ExtraPrice)> Items)>> GetForMenuProductWithPricesAsync(int menuProductId, CancellationToken cancellationToken = default)
    {
        var menuModifiers = await db.MenuModifiers
            .AsNoTracking()
            .Where(mm => mm.MenuProductId == menuProductId && mm.IsAvailable)
            .Include(mm => mm.Modifier)
                .ThenInclude(m => m.ModifierGroup)
            .ToListAsync(cancellationToken);

        return menuModifiers
            .GroupBy(mm => mm.Modifier.ModifierGroup)
            .Select(g => (
                Group: g.Key,
                Items: g.Select(mm => (mm.Modifier, mm.ExtraPrice)).ToList()
            ))
            .ToList();
    }

    public void Add(ModifierGroup group) => db.ModifierGroups.Add(group);

    public void Remove(ModifierGroup group) => db.ModifierGroups.Remove(group);
}
