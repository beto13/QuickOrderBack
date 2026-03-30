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

    public void Add(ModifierGroup group) => db.ModifierGroups.Add(group);

    public void Remove(ModifierGroup group) => db.ModifierGroups.Remove(group);
}
