using GoldenBread.Domain.Entities;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace GoldenBread.Infrastructure.Data.Configurations;

public class IngredientBatchConfiguration : IEntityTypeConfiguration<IngredientBatch>
{
    public void Configure(EntityTypeBuilder<IngredientBatch> builder)
    {
        builder.HasKey(e => e.IngredientBatchId).HasName("ingredient_batches_pkey");

        builder.HasIndex(e => e.SupplierIngredientId, "idx_ingredient_batches_supplier_ingredient");

        builder.Property(e => e.RemainingQuantity).HasPrecision(10, 3);

        builder.HasOne(d => d.SupplierIngredient)
            .WithMany(p => p.IngredientBatches)
            .HasForeignKey(d => d.SupplierIngredientId)
            .OnDelete(DeleteBehavior.ClientSetNull)
            .HasConstraintName("fk_ingredient_batches_supplier_ingredient");

        builder.HasMany(d => d.OrderItemReservations)
            .WithOne(p => p.IngredientBatch)
            .HasForeignKey(p => p.IngredientBatchId)
            .OnDelete(DeleteBehavior.Restrict);
    }
}
