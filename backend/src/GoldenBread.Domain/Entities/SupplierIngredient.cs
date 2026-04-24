using GoldenBread.Domain.Enums;

namespace GoldenBread.Domain.Entities;

public class SupplierIngredient
{
    public int SupplierIngredientId { get; set; }

    public int SupplierId { get; set; }
    public int IngredientId { get; set; }

    public string Name { get; set; } = null!;
    public decimal Price { get; set; }
    public IngredientUnit Unit { get; set; }
    public decimal Weight { get; set; }
    public int ShelfLifeMonths { get; set; }
    public DateTime? DeletedAt { get; set; }

    public Supplier Supplier { get; set; } = null!;
    public Ingredient Ingredient { get; set; } = null!;
    public ICollection<IngredientBatch> IngredientBatches { get; set; } = new List<IngredientBatch>();
}
