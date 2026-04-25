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
