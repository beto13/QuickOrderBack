using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using QuickOrder.Domain.Entities;

namespace QuickOrder.Infrastructure.Persistence.Configurations;

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
