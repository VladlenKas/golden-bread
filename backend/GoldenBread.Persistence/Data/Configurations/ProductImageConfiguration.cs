using GoldenBread.Domain.Entities;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace GoldenBread.Infrastructure.Data.Configurations;

public class ProductImageConfiguration : IEntityTypeConfiguration<ProductImage>
{
    public void Configure(EntityTypeBuilder<ProductImage> builder)
    {
        builder.HasKey(e => e.ProductImageId).HasName("product_images_pkey");

        builder.HasIndex(e => e.ProductId, "fk_product_images_product_id_idx");

        builder.HasOne(d => d.Product)
            .WithMany(p => p.ProductImages)
            .HasForeignKey(d => d.ProductId)
            .OnDelete(DeleteBehavior.ClientSetNull)
            .HasConstraintName("fk_product_images_product_id");
    }
}
    