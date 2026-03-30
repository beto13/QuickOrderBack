using Microsoft.EntityFrameworkCore;
using QuickOrder.Application.Interfaces;
using QuickOrder.Domain.Entities;
using QuickOrder.Infrastructure.Persistence;

namespace QuickOrder.Infrastructure.Repositories;

public class MenuRepository(AppDbContext db) : IMenuRepository
{
    public async Task<(List<Menu> Items, int TotalCount)> GetPagedAsync(int pageNumber, int pageSize, CancellationToken cancellationToken = default)
    {
        var query = db.Menus.AsNoTracking().OrderBy(m => m.Name);
        var total = await query.CountAsync(cancellationToken);
        var items = await query
            .Skip((pageNumber - 1) * pageSize)
            .Take(pageSize)
            .ToListAsync(cancellationToken);
        return (items, total);
    }

    public Task<Menu?> FindByIdAsync(int id, CancellationToken cancellationToken = default) =>
        db.Menus.FindAsync([id], cancellationToken).AsTask();

    public void Add(Menu menu) => db.Menus.Add(menu);
}
