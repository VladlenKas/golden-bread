using GoldenBread.Domain.Entities;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace GoldenBread.Infrastructure.Data.Configurations;

public class IngredientReservationConfiguration : IEntityTypeConfiguration<IngredientReservation>
{
    public void Configure(EntityTypeBuilder<IngredientReservation> builder)
    {
        builder.ToTable("ingredient_reservations");

        builder.HasKey(ir => ir.IngredientReservationId);

        builder.HasIndex(ir => ir.OrderId);
        builder.HasIndex(ir => ir.IngredientBatchId);
        builder.HasIndex(ir => new { ir.OrderId, ir.IsActive });

        builder.HasOne(ir => ir.Order)
            .WithMany()
            .HasForeignKey(ir => ir.OrderId)
            .OnDelete(DeleteBehavior.Restrict);

        builder.HasOne(ir => ir.Batch)
            .WithMany()
            .HasForeignKey(ir => ir.IngredientBatchId)
            .OnDelete(DeleteBehavior.Restrict);

        builder.Property(ir => ir.OrderId).IsRequired();
        builder.Property(ir => ir.IngredientBatchId).IsRequired();
        builder.Property(ir => ir.ReservedQuantity).IsRequired().HasPrecision(18, 3); 
        builder.Property(ir => ir.ReservedAt).IsRequired();
        builder.Property(ir => ir.IsActive).IsRequired().HasDefaultValue(true);
        builder.Property(ir => ir.IsConfirmed).IsRequired().HasDefaultValue(false);
    }
}
