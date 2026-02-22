using GoldenBread.Domain.Entities;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace GoldenBread.Infrastructure.Configurations;

public class ProductBatchConfiguration : IEntityTypeConfiguration<ProductBatch>
{
    public void Configure(EntityTypeBuilder<ProductBatch> builder)
    {
        builder.HasKey(e => e.ProductBatchId).HasName("product_batches_new_pkey");
        builder.ToTable("product_batches");

        builder.HasIndex(e => e.ProductId, "fk_product_batches_product_id_idx");

        builder.Property(e => e.ProductBatchId)
            .HasColumnName("product_batch_id");

        builder.Property(e => e.ProductId)
            .HasColumnName("product_id");

        builder.Property(e => e.QuantityPerBatch)
            .HasColumnName("units");

        builder.HasOne(d => d.Product)
            .WithMany(p => p.ProductBatches)
            .HasForeignKey(d => d.ProductId)
            .OnDelete(DeleteBehavior.ClientSetNull)
            .HasConstraintName("fk_product_batches_product_id");
    }
}
