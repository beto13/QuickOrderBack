using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using QuickOrder.Domain.Entities;

namespace QuickOrder.Infrastructure.Persistence.Configurations;

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
