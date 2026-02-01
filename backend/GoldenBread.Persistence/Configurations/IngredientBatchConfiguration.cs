using GoldenBread.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GoldenBread.Infrastructure.Configurations;

public class IngredientBatchConfiguration : IEntityTypeConfiguration<IngredientBatch>
{
    public void Configure(EntityTypeBuilder<IngredientBatch> builder)
    {
        builder.HasKey(e => e.IngredientBatchId).HasName("ingredient_batches_pkey");

        builder.ToTable("ingredient_batches");

        builder.HasIndex(e => e.IngredientId, "fk_ingredient_batches_ingredient_id_idx");

        builder.Property(e => e.IngredientBatchId).HasColumnName("ingredient_batch_id");
        builder.Property(e => e.DeliveryDate).HasColumnName("delivery_date");
        builder.Property(e => e.ExpiryDate).HasColumnName("expiry_date");
        builder.Property(e => e.IngredientId).HasColumnName("ingredient_id");
        builder.Property(e => e.PurchasedQuantity).HasColumnName("purchased_quantity");
        builder.Property(e => e.RemainingQuantity)
            .HasPrecision(4, 3)
            .HasColumnName("remaining_quantity");

        builder.HasOne(d => d.Ingredient).WithMany(p => p.IngredientBatches)
            .HasForeignKey(d => d.IngredientId)
            .OnDelete(DeleteBehavior.ClientSetNull)
            .HasConstraintName("fk_ingredient_batches_ingredient_id");
    }
}