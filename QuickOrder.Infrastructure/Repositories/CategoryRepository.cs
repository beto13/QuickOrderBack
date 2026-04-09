using Microsoft.EntityFrameworkCore;
using QuickOrder.Application.Interfaces;
using QuickOrder.Domain.Entities;
using QuickOrder.Infrastructure.Persistence;

namespace QuickOrder.Infrastructure.Repositories;

public class CategoryRepository(AppDbContext db) : ICategoryRepository
{
    public Task<List<Category>> GetAllWithMenuProductsAsync(int menuId, CancellationToken cancellationToken = default) =>
        db.Categories
            .Include(c => c.MenuProducts.Where(mp => mp.MenuId == menuId && mp.IsAvailable))
                .ThenInclude(mp => mp.Product)
            .Include(c => c.MenuProducts.Where(mp => mp.MenuId == menuId && mp.IsAvailable))
                .ThenInclude(mp => mp.MenuModifiers.Where(mm => mm.IsAvailable))
            .Where(c => c.MenuProducts.Any(mp => mp.MenuId == menuId && mp.IsAvailable))
            .OrderBy(c => c.DisplayOrder)
            .AsSplitQuery()
            .ToListAsync(cancellationToken);

    public async Task<(List<Category> Items, int TotalCount)> GetPagedAsync(int pageNumber, int pageSize, CancellationToken cancellationToken = default)
    {
        var query = db.Categories.AsNoTracking().OrderBy(c => c.DisplayOrder).ThenBy(c => c.Name);
        var total = await query.CountAsync(cancellationToken);
        var items = await query
            .Skip((pageNumber - 1) * pageSize)
            .Take(pageSize)
            .ToListAsync(cancellationToken);
        return (items, total);
    }

    public Task<Category?> FindByIdAsync(int id, CancellationToken cancellationToken = default) =>
        db.Categories.FindAsync([id], cancellationToken).AsTask();

    public void Add(Category category) => db.Categories.Add(category);

    public void Remove(Category category) => db.Categories.Remove(category);
}
