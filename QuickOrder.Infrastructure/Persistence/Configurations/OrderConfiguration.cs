using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using QuickOrder.Domain.Entities;

namespace QuickOrder.Infrastructure.Persistence.Configurations;

public class OrderConfiguration : IEntityTypeConfiguration<Order>
{
    public void Configure(EntityTypeBuilder<Order> builder)
    {
        builder.Property(o => o.Status).HasConversion<string>();
        builder.Property(o => o.Notes).HasMaxLength(500);
        builder.HasOne(o => o.Menu)
               .WithMany(m => m.Orders)
               .HasForeignKey(o => o.MenuId)
               .OnDelete(DeleteBehavior.Restrict);
    }
}

public class ProductConfiguration : IEntityTypeConfiguration<Product>
{
    public void Configure(EntityTypeBuilder<Product> builder)
    {
        builder.Property(p => p.Name).HasMaxLength(150).IsRequired();
        builder.Property(p => p.Description).HasMaxLength(500);
    }
}

public class OrderItemConfiguration : IEntityTypeConfiguration<OrderItem>
{
    public void Configure(EntityTypeBuilder<OrderItem> builder)
    {
        builder.Property(i => i.UnitPrice).HasPrecision(10, 2);
        builder.Property(i => i.Notes).HasMaxLength(300);
        builder.HasOne(i => i.MenuProduct)
               .WithMany(mp => mp.OrderItems)
               .HasForeignKey(i => i.MenuProductId)
               .OnDelete(DeleteBehavior.Restrict);
    }
}

public class TableConfiguration : IEntityTypeConfiguration<Table>
{
    public void Configure(EntityTypeBuilder<Table> builder)
    {
        builder.Property(t => t.Number).HasMaxLength(10).IsRequired();
    }
}

public class CategoryConfiguration : IEntityTypeConfiguration<Category>
{
    public void Configure(EntityTypeBuilder<Category> builder)
    {
        builder.Property(c => c.Name).HasMaxLength(100).IsRequired();
    }
}

public class OrderStatusHistoryConfiguration : IEntityTypeConfiguration<OrderStatusHistory>
{
    public void Configure(EntityTypeBuilder<OrderStatusHistory> builder)
    {
        builder.Property(h => h.FromStatus).HasMaxLength(50).IsRequired();
        builder.Property(h => h.ToStatus).HasMaxLength(50).IsRequired();
        builder.HasOne(h => h.Order)
               .WithMany(o => o.StatusHistory)
               .HasForeignKey(h => h.OrderId)
               .OnDelete(DeleteBehavior.Cascade);
    }
}

public class MenuConfiguration : IEntityTypeConfiguration<Menu>
{
    public void Configure(EntityTypeBuilder<Menu> builder)
    {
        builder.Property(m => m.Name).HasMaxLength(100).IsRequired();
    }
}

public class MenuProductConfiguration : IEntityTypeConfiguration<MenuProduct>
{
    public void Configure(EntityTypeBuilder<MenuProduct> builder)
    {
        builder.Property(mp => mp.Price).HasPrecision(10, 2);
        builder.HasOne(mp => mp.Menu)
               .WithMany(m => m.MenuProducts)
               .HasForeignKey(mp => mp.MenuId)
               .OnDelete(DeleteBehavior.Cascade);
        builder.HasOne(mp => mp.Product)
               .WithMany(p => p.MenuProducts)
               .HasForeignKey(mp => mp.ProductId)
               .OnDelete(DeleteBehavior.Restrict);
        builder.HasOne(mp => mp.Category)
               .WithMany(c => c.MenuProducts)
               .HasForeignKey(mp => mp.CategoryId)
               .OnDelete(DeleteBehavior.Restrict);
    }
}

public class ModifierGroupConfiguration : IEntityTypeConfiguration<ModifierGroup>
{
    public void Configure(EntityTypeBuilder<ModifierGroup> builder)
    {
        builder.Property(mg => mg.Name).HasMaxLength(100).IsRequired();
        builder.HasOne(mg => mg.Product)
               .WithMany(p => p.ModifierGroups)
               .HasForeignKey(mg => mg.ProductId)
               .OnDelete(DeleteBehavior.Cascade)
               .IsRequired(false);
        builder.HasOne(mg => mg.Category)
               .WithMany(c => c.ModifierGroups)
               .HasForeignKey(mg => mg.CategoryId)
               .OnDelete(DeleteBehavior.Cascade)
               .IsRequired(false);
    }
}

public class ModifierConfiguration : IEntityTypeConfiguration<Modifier>
{
    public void Configure(EntityTypeBuilder<Modifier> builder)
    {
        builder.Property(m => m.Name).HasMaxLength(150).IsRequired();
        builder.Property(m => m.Description).HasMaxLength(500);
        builder.HasOne(m => m.ModifierGroup)
               .WithMany(mg => mg.Modifiers)
               .HasForeignKey(m => m.ModifierGroupId)
               .OnDelete(DeleteBehavior.Cascade);
    }
}

public class MenuModifierConfiguration : IEntityTypeConfiguration<MenuModifier>
{
    public void Configure(EntityTypeBuilder<MenuModifier> builder)
    {
        builder.Property(mm => mm.ExtraPrice).HasPrecision(10, 2);
        builder.HasOne(mm => mm.MenuProduct)
               .WithMany(mp => mp.MenuModifiers)
               .HasForeignKey(mm => mm.MenuProductId)
               .OnDelete(DeleteBehavior.Cascade);
        builder.HasOne(mm => mm.Modifier)
               .WithMany(m => m.MenuModifiers)
               .HasForeignKey(mm => mm.ModifierId)
               .OnDelete(DeleteBehavior.Restrict);
    }
}

public class OrderItemModifierConfiguration : IEntityTypeConfiguration<OrderItemModifier>
{
    public void Configure(EntityTypeBuilder<OrderItemModifier> builder)
    {
        builder.Property(oim => oim.ExtraPrice).HasPrecision(10, 2);
        builder.HasOne(oim => oim.OrderItem)
               .WithMany(oi => oi.Modifiers)
               .HasForeignKey(oim => oim.OrderItemId)
               .OnDelete(DeleteBehavior.Cascade);
        builder.HasOne(oim => oim.Modifier)
               .WithMany(m => m.OrderItemModifiers)
               .HasForeignKey(oim => oim.ModifierId)
               .OnDelete(DeleteBehavior.Restrict);
    }
}
