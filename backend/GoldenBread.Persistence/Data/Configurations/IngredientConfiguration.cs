using GoldenBread.Domain.Entities;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace GoldenBread.Infrastructure.Data.Configurations;

public class IngredientConfiguration : IEntityTypeConfiguration<Ingredient>
{
    public void Configure(EntityTypeBuilder<Ingredient> builder)
    {
        builder.HasKey(e => e.IngredientId).HasName("ingredients_pkey");

        builder.HasQueryFilter(e => e.DeletedAt == null);

        builder.HasIndex(e => e.SupplierId, "fk_ingredients_supplier_id_idx");

        builder.Property(e => e.Name).HasMaxLength(100);
        builder.Property(e => e.Price).HasPrecision(10, 2);
        builder.Property(e => e.Weight).HasPrecision(10, 3);

        builder.HasOne(d => d.Supplier)
            .WithMany(p => p.Ingredients)
            .HasForeignKey(d => d.SupplierId)
            .OnDelete(DeleteBehavior.ClientSetNull)
            .HasConstraintName("fk_ingredients_supplier_id");
    }
}
