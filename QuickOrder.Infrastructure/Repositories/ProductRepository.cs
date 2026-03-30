using Microsoft.EntityFrameworkCore;
using QuickOrder.Application.Interfaces;
using QuickOrder.Domain.Entities;
using QuickOrder.Infrastructure.Persistence;

namespace QuickOrder.Infrastructure.Repositories;

public class ProductRepository(AppDbContext db) : IProductRepository
{
    public async Task<(List<Product> Items, int TotalCount)> GetPagedAsync(int pageNumber, int pageSize, CancellationToken cancellationToken = default)
    {
        var query = db.Products.AsNoTracking().OrderBy(p => p.Name);
        var total = await query.CountAsync(cancellationToken);
        var items = await query
            .Skip((pageNumber - 1) * pageSize)
            .Take(pageSize)
            .ToListAsync(cancellationToken);
        return (items, total);
    }

    public Task<Product?> FindByIdAsync(int id, CancellationToken cancellationToken = default) =>
        db.Products.FindAsync([id], cancellationToken).AsTask();

    public void Add(Product product) => db.Products.Add(product);

    public void Remove(Product product) => db.Products.Remove(product);
}
