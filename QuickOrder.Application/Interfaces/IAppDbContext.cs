using Microsoft.EntityFrameworkCore;
using QuickOrder.Domain.Entities;

namespace QuickOrder.Application.Interfaces;

public interface IAppDbContext
{
    DbSet<Table> Tables { get; }
    DbSet<Category> Categories { get; }
    DbSet<Product> Products { get; }
    DbSet<Menu> Menus { get; }
    DbSet<MenuProduct> MenuProducts { get; }
    DbSet<ModifierGroup> ModifierGroups { get; }
    DbSet<Modifier> Modifiers { get; }
    DbSet<MenuModifier> MenuModifiers { get; }
    DbSet<Order> Orders { get; }
    DbSet<OrderItem> OrderItems { get; }
    DbSet<OrderItemModifier> OrderItemModifiers { get; }
    DbSet<OrderStatusHistory> OrderStatusHistories { get; }

    Task<int> SaveChangesAsync(CancellationToken cancellationToken = default);
}
