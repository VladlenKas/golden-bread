using GoldenBread.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GoldenBread.Infrastructure.Configurations;

public class RecipeConfiguration : IEntityTypeConfiguration<Recipe>
{
    public void Configure(EntityTypeBuilder<Recipe> builder)
    {
        builder.HasKey(e => e.RecipeId).HasName("recipes_pkey");

        builder.ToTable("recipes");

        builder.HasIndex(e => e.IngredientId, "fk_recipe_ingredient_id_idx");

        builder.HasIndex(e => e.ProductId, "fk_recipe_product_id_idx");

        builder.Property(e => e.RecipeId).HasColumnName("recipe_id");
        builder.Property(e => e.IngredientId).HasColumnName("ingredient_id");
        builder.Property(e => e.ProductId).HasColumnName("product_id");
        builder.Property(e => e.Quantity)
            .HasPrecision(4, 3)
            .HasColumnName("quantity");

        builder.HasOne(d => d.Ingredient).WithMany(p => p.Recipes)
            .HasForeignKey(d => d.IngredientId)
            .OnDelete(DeleteBehavior.ClientSetNull)
            .HasConstraintName("fk_recipe_ingredient_id");

        builder.HasOne(d => d.Product).WithMany(p => p.Recipes)
            .HasForeignKey(d => d.ProductId)
            .OnDelete(DeleteBehavior.ClientSetNull)
            .HasConstraintName("fk_recipe_product_id");
    }
}
