using GoldenBread.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GoldenBread.Infrastructure.Configurations;

public class ProductCategoryConfiguration : IEntityTypeConfiguration<ProductCategory>
{
    public void Configure(EntityTypeBuilder<ProductCategory> builder)
    {
        builder.HasKey(e => e.ProductCategoryId).HasName("production_categories_pkey");

        builder.ToTable("product_categories");

        builder.Property(e => e.ProductCategoryId)
            .HasDefaultValueSql("nextval('production_categories_production_category_id_seq'::regclass)")
            .HasColumnName("product_category_id");
        builder.Property(e => e.Color)
            .HasMaxLength(6)
            .HasColumnName("color");
        builder.Property(e => e.IsDelete)
            .HasDefaultValue((short)0)
            .HasColumnName("is_delete");
        builder.Property(e => e.Icon).HasColumnName("icon");
        builder.Property(e => e.Image).HasColumnName("image");
        builder.Property(e => e.Name)
            .HasMaxLength(100)
            .HasColumnName("name");

        builder.HasQueryFilter(e => e.IsDelete == 0);
    }
}