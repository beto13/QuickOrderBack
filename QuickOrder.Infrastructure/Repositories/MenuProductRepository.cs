using Microsoft.EntityFrameworkCore;
using QuickOrder.Application.Interfaces;
using QuickOrder.Domain.Entities;
using QuickOrder.Infrastructure.Persistence;

namespace QuickOrder.Infrastructure.Repositories;

public class MenuProductRepository(AppDbContext db) : IMenuProductRepository
{
    public Task<Dictionary<int, MenuProduct>> GetByIdsAsync(IEnumerable<int> menuProductIds, CancellationToken cancellationToken = default) =>
        db.MenuProducts
            .Include(mp => mp.Product)
            .Include(mp => mp.Menu)
            .Where(mp => menuProductIds.Contains(mp.Id) && mp.IsAvailable)
            .ToDictionaryAsync(mp => mp.Id, cancellationToken);
}
