using GoldenBread.Domain.Entities;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace GoldenBread.Infrastructure.Data.Configurations;

public class ProductCategoryConfiguration : IEntityTypeConfiguration<ProductCategory>
{
    public void Configure(EntityTypeBuilder<ProductCategory> builder)
    {
        builder.HasKey(e => e.ProductCategoryId).HasName("production_categories_pkey");

        builder.HasQueryFilter(e => e.DeletedAt == null);

        builder.Property(e => e.Color).HasMaxLength(6);
        builder.Property(e => e.Name).HasMaxLength(100);
    }
}
