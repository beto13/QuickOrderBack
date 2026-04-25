using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using QuickOrder.Domain.Entities;

namespace QuickOrder.Infrastructure.Persistence.Configurations;

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
