using Microsoft.EntityFrameworkCore;
using QuickOrder.Application.Interfaces;
using QuickOrder.Domain.Entities;
using QuickOrder.Infrastructure.Persistence;

namespace QuickOrder.Infrastructure.Repositories;

public class ModifierRepository(AppDbContext db) : IModifierRepository
{
    public async Task<(List<Modifier> Items, int TotalCount)> GetPagedAsync(int modifierGroupId, int pageNumber, int pageSize, CancellationToken cancellationToken = default)
    {
        var query = db.Modifiers.AsNoTracking()
            .Where(m => m.ModifierGroupId == modifierGroupId)
            .OrderBy(m => m.Name);

        var total = await query.CountAsync(cancellationToken);
        var items = await query
            .Skip((pageNumber - 1) * pageSize)
            .Take(pageSize)
            .ToListAsync(cancellationToken);
        return (items, total);
    }

    public Task<Modifier?> FindByIdAsync(int id, CancellationToken cancellationToken = default) =>
        db.Modifiers.FindAsync([id], cancellationToken).AsTask();

    public void Add(Modifier modifier) => db.Modifiers.Add(modifier);

    public void Remove(Modifier modifier) => db.Modifiers.Remove(modifier);
}
