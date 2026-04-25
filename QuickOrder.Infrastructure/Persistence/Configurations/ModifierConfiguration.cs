using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using QuickOrder.Domain.Entities;

namespace QuickOrder.Infrastructure.Persistence.Configurations;

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
