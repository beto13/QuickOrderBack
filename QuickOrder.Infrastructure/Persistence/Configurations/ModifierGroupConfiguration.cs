using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using QuickOrder.Domain.Entities;

namespace QuickOrder.Infrastructure.Persistence.Configurations;

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
