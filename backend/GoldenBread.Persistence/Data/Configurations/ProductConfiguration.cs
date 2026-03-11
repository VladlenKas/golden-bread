using GoldenBread.Domain.Entities;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace GoldenBread.Infrastructure.Data.Configurations;

public class ProductConfiguration : IEntityTypeConfiguration<Product>
{
    public void Configure(EntityTypeBuilder<Product> builder)
    {
        builder.HasKey(e => e.ProductId).HasName("products_pkey");

        builder.HasQueryFilter(e => e.DeletedAt == null);

        builder.HasIndex(e => e.CategoryId, "fk_products_category_id_idx");

        builder.Property(e => e.CostPrice).HasPrecision(4, 2);
        builder.Property(e => e.Name).HasMaxLength(100);
        builder.Property(e => e.Weight).HasPrecision(5, 3);
        builder.Property(e => e.StorageTempMin).HasPrecision(4, 1);
        builder.Property(e => e.StorageTempMax).HasPrecision(4, 1);

        builder.HasOne(d => d.Category)
            .WithMany(p => p.Products)
            .HasForeignKey(d => d.CategoryId)
            .OnDelete(DeleteBehavior.ClientSetNull)
            .HasConstraintName("fk_products_category_id");
    }
}
