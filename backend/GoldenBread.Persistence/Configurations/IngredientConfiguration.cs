using GoldenBread.Domain.Entities;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace GoldenBread.Infrastructure.Configurations;

public class IngredientConfiguration : IEntityTypeConfiguration<Ingredient>
{
    public void Configure(EntityTypeBuilder<Ingredient> builder)
    {
        builder.HasKey(e => e.IngredientId).HasName("ingredients_pkey");
        builder.ToTable("ingredients");

        builder.HasIndex(e => e.SupplierId, "fk_ingredients_supplier_id_idx");

        builder.HasQueryFilter(e => e.DeletedAt == null);

        builder.Property(e => e.IngredientId)
            .HasColumnName("ingredient_id");

        builder.Property(e => e.DeletedAt)
            .HasColumnName("deleted_at");

        builder.Property(e => e.Name)
            .HasMaxLength(100)
            .HasColumnName("name");

        builder.Property(e => e.Price)
            .HasPrecision(10, 2)
            .HasColumnName("price");

        builder.Property(e => e.ShelfLifeMonths)
            .HasColumnName("shelf_life_months");

        builder.Property(e => e.SupplierId)
            .HasColumnName("supplier_id");

        builder.Property(e => e.Weight)
            .HasPrecision(10, 3)
            .HasColumnName("weight");

        builder.HasOne(d => d.Supplier)
            .WithMany(p => p.Ingredients)
            .HasForeignKey(d => d.SupplierId)
            .OnDelete(DeleteBehavior.ClientSetNull)
            .HasConstraintName("fk_ingredients_supplier_id");
    }
}
