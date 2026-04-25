using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using QuickOrder.Domain.Entities;

namespace QuickOrder.Infrastructure.Persistence.Configurations;

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
