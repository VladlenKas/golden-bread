using GoldenBread.Domain.Enums;

namespace GoldenBread.Domain.Entities;

public class Ingredient
{
    public int IngredientId { get; set; }

    public string Name { get; set; } = null!;
    public DateTime? DeletedAt { get; set; }

    public IngredientUnit BaseUnit { get; set; }

    public ICollection<SupplierIngredient> SupplierIngredients { get; set; } = new List<SupplierIngredient>();
    public ICollection<Recipe> Recipes { get; set; } = new List<Recipe>();

    public void SoftDelete()
    {
        DeletedAt = DateTime.UtcNow;
    }
}
