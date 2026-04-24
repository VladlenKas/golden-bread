using GoldenBread.Domain.Entities;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace GoldenBread.Infrastructure.Data.Configurations;

public class SupplierIngredientConfiguration : IEntityTypeConfiguration<SupplierIngredient>
{
    public void Configure(EntityTypeBuilder<SupplierIngredient> builder)
    {
        builder.HasKey(e => e.SupplierIngredientId).HasName("supplier_ingredient_pkey");

        builder.HasQueryFilter(e => e.DeletedAt == null);

        builder.HasIndex(e => e.IngredientId, "idx_supplier_ingredient_ingredient");
        builder.HasIndex(e => e.SupplierId, "idx_supplier_ingredient_supplier");

        builder.Property(e => e.Name).HasMaxLength(100);
        builder.Property(e => e.Price).HasPrecision(10, 2);
        builder.Property(e => e.Weight).HasPrecision(10, 6);

        builder.HasOne(e => e.Supplier)
            .WithMany(s => s.SupplierIngredients)
            .HasForeignKey(e => e.SupplierId)
            .OnDelete(DeleteBehavior.NoAction)
            .HasConstraintName("fk_supplier_ingredient_supplier");

        builder.HasOne(e => e.Ingredient)
            .WithMany(i => i.SupplierIngredients)
            .HasForeignKey(e => e.IngredientId)
            .OnDelete(DeleteBehavior.NoAction)
            .HasConstraintName("fk_supplier_ingredient_ingredient");

        builder.HasMany(e => e.IngredientBatches)
            .WithOne(ib => ib.SupplierIngredient)
            .HasForeignKey(ib => ib.SupplierIngredientId)
            .OnDelete(DeleteBehavior.NoAction)
            .HasConstraintName("fk_ingredient_batches_supplier_ingredient");
    }
}