using GoldenBread.Domain.Entities;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace GoldenBread.Infrastructure.Data.Configurations;

public class IngredientBatchConfiguration : IEntityTypeConfiguration<IngredientBatch>
{
    public void Configure(EntityTypeBuilder<IngredientBatch> builder)
    {
        builder.HasKey(e => e.IngredientBatchId).HasName("ingredient_batches_pkey");

        builder.HasIndex(e => e.IngredientId, "fk_ingredient_batches_ingredient_id_idx");

        builder.Property(e => e.RemainingQuantity).HasPrecision(10, 3);

        builder.HasOne(d => d.Ingredient)
            .WithMany(p => p.IngredientBatches)
            .HasForeignKey(d => d.IngredientId)
            .OnDelete(DeleteBehavior.ClientSetNull)
            .HasConstraintName("fk_ingredient_batches_ingredient_id");
    }
}
