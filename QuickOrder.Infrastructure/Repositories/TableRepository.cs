using Microsoft.EntityFrameworkCore;
using QuickOrder.Application.Interfaces;
using QuickOrder.Domain.Entities;
using QuickOrder.Infrastructure.Persistence;

namespace QuickOrder.Infrastructure.Repositories;

public class TableRepository(AppDbContext db) : ITableRepository
{
    public async Task<(List<Table> Items, int TotalCount)> GetPagedAsync(int pageNumber, int pageSize, CancellationToken cancellationToken = default)
    {
        var query = db.Tables.AsNoTracking().OrderBy(t => t.Number);
        var total = await query.CountAsync(cancellationToken);
        var items = await query
            .Skip((pageNumber - 1) * pageSize)
            .Take(pageSize)
            .ToListAsync(cancellationToken);
        return (items, total);
    }

    public Task<Table?> FindByIdAsync(int id, CancellationToken cancellationToken = default) =>
        db.Tables.FindAsync([id], cancellationToken).AsTask();

    public Task<Table?> FindByNumberAsync(string number, CancellationToken cancellationToken = default) =>
        db.Tables.AsNoTracking().FirstOrDefaultAsync(t => t.Number == number && t.IsActive, cancellationToken);

    public void Add(Table table) => db.Tables.Add(table);
}
