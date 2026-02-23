using GoldenBread.Domain.Enums;

namespace GoldenBread.Domain.Entities;

public class Ingredient
{
    public int IngredientId { get; set; }

    public int SupplierId { get; set; }

    public string Name { get; set; } = null!;
    public decimal Price { get; set; }
    public decimal Weight { get; set; }
    public int ShelfLifeMonths { get; set; }
    public DateTime? DeletedAt { get; set; }

    public IngredientUnit Unit { get; set; }

    public ICollection<IngredientBatch> IngredientBatches { get; set; } = new List<IngredientBatch>();
    public ICollection<Recipe> Recipes { get; set; } = new List<Recipe>();

    public Supplier Supplier { get; set; } = null!;
}
