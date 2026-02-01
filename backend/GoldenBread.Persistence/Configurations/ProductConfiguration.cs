using GoldenBread.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GoldenBread.Infrastructure.Configurations;

public class ProductConfiguration : IEntityTypeConfiguration<Product>
{
    public void Configure(EntityTypeBuilder<Product> builder)
    {
        builder.HasKey(e => e.ProductId).HasName("products_pkey");

        builder.ToTable("products");

        builder.HasIndex(e => e.CategoryId, "fk_products_category_id_idx");

        builder.Property(e => e.ProductId).HasColumnName("product_id");
        builder.Property(e => e.CategoryId).HasColumnName("category_id");
        builder.Property(e => e.CostPrice)
            .HasPrecision(4, 2)
            .HasColumnName("cost_price");
        builder.Property(e => e.IsDelete)
            .HasDefaultValue((short)0)
            .HasColumnName("is_delete");
        builder.Property(e => e.Description).HasColumnName("description");
        builder.Property(e => e.MarkupPercent).HasColumnName("markup_percent");
        builder.Property(e => e.Name)
            .HasMaxLength(100)
            .HasColumnName("name");
        builder.Property(e => e.ProductionTime).HasColumnName("production_time");
        builder.Property(e => e.SalePrice)
            .HasPrecision(4, 2)
            .HasColumnName("sale_price");
        builder.Property(e => e.Weight)
            .HasPrecision(5, 3)
            .HasColumnName("weight");

        builder.HasOne(d => d.Category).WithMany(p => p.Products)
            .HasForeignKey(d => d.CategoryId)
            .OnDelete(DeleteBehavior.ClientSetNull)
            .HasConstraintName("fk_products_category_id");

        builder.HasQueryFilter(e => e.IsDelete == 0);
    }
}
