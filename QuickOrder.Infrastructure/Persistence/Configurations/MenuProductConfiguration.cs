using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using QuickOrder.Domain.Entities;

namespace QuickOrder.Infrastructure.Persistence.Configurations;

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
