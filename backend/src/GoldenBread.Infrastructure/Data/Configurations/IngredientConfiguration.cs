using GoldenBread.Domain.Entities;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace GoldenBread.Infrastructure.Data.Configurations;

public class IngredientConfiguration : IEntityTypeConfiguration<Ingredient>
{
    public void Configure(EntityTypeBuilder<Ingredient> builder)
    {
        builder.HasKey(e => e.IngredientId).HasName("ingredients_pkey");

        builder.HasQueryFilter(e => e.DeletedAt == null);

        builder.Property(e => e.Name).HasMaxLength(100);

        builder.HasMany(e => e.Recipes)
            .WithOne(r => r.Ingredient)
            .HasForeignKey(r => r.IngredientId)
            .OnDelete(DeleteBehavior.NoAction)
            .HasConstraintName("fk_recipe_ingredient_id");
    }
}
