using Microsoft.EntityFrameworkCore;
using QuickOrder.Application.Interfaces;
using QuickOrder.Domain.Entities;
using QuickOrder.Domain.Enums;
using QuickOrder.Infrastructure.Persistence;

namespace QuickOrder.Infrastructure.Repositories;

public class OrderRepository(AppDbContext db) : IOrderRepository
{
    public Task<Order?> FindByIdAsync(int id, CancellationToken cancellationToken = default) =>
        db.Orders.FindAsync([id], cancellationToken).AsTask();

    public Task<Order?> GetByIdWithDetailsAsync(int id, CancellationToken cancellationToken = default) =>
        db.Orders
            .Include(o => o.Table)
            .Include(o => o.Menu)
            .Include(o => o.Items).ThenInclude(i => i.MenuProduct).ThenInclude(mp => mp.Product)
            .AsNoTracking()
            .FirstOrDefaultAsync(o => o.Id == id, cancellationToken);

    public Task<List<Order>> GetActiveAsync(CancellationToken cancellationToken = default) =>
        db.Orders
            .AsNoTracking()
            .Include(o => o.Table)
            .Include(o => o.Menu)
            .Include(o => o.Items).ThenInclude(i => i.MenuProduct).ThenInclude(mp => mp.Product)
            .Where(o => o.Status != OrderStatus.Delivered && o.Status != OrderStatus.Cancelled)
            .OrderBy(o => o.CreatedAt)
            .ToListAsync(cancellationToken);

    public Task<List<Order>> GetAllAsync(CancellationToken cancellationToken = default) =>
        db.Orders
            .AsNoTracking()
            .Include(o => o.Table)
            .Include(o => o.Menu)
            .Include(o => o.Items).ThenInclude(i => i.MenuProduct).ThenInclude(mp => mp.Product)
            .Include(o => o.StatusHistory)
            .OrderByDescending(o => o.CreatedAt)
            .ToListAsync(cancellationToken);

    public async Task<(List<Order> Items, int TotalCount)> GetPagedAsync(int pageNumber, int pageSize, CancellationToken cancellationToken = default)
    {
        var query = db.Orders
            .AsNoTracking()
            .Include(o => o.Table)
            .Include(o => o.Menu)
            .Include(o => o.Items).ThenInclude(i => i.MenuProduct).ThenInclude(mp => mp.Product)
            .Include(o => o.StatusHistory)
            .OrderByDescending(o => o.CreatedAt);

        var total = await query.CountAsync(cancellationToken);
        var items = await query
            .Skip((pageNumber - 1) * pageSize)
            .Take(pageSize)
            .ToListAsync(cancellationToken);

        return (items, total);
    }

    public void Add(Order order) => db.Orders.Add(order);

    public void AddStatusHistory(OrderStatusHistory history) => db.OrderStatusHistories.Add(history);
}
