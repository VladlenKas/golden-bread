using GoldenBread.Domain.Entities;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace GoldenBread.Infrastructure.Configurations;

public class ProductImageConfiguration : IEntityTypeConfiguration<ProductImage>
{
    public void Configure(EntityTypeBuilder<ProductImage> builder)
    {
        builder.HasKey(e => e.ProductImageId).HasName("product_images_pkey");

        builder.ToTable("product_images");

        builder.HasIndex(e => e.ProductId, "fk_product_images_product_id_idx");

        builder.Property(e => e.ProductImageId).HasColumnName("product_image_id");
        builder.Property(e => e.Image).HasColumnName("image");
        builder.Property(e => e.ProductId).HasColumnName("product_id");

        builder.HasOne(d => d.Product).WithMany(p => p.ProductImages)
            .HasForeignKey(d => d.ProductId)
            .OnDelete(DeleteBehavior.ClientSetNull)
            .HasConstraintName("fk_product_images_product_id");
    }
}
