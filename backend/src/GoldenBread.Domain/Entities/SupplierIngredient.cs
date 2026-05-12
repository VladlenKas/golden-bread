using GoldenBread.Domain.Enums;

namespace GoldenBread.Domain.Entities;

public class SupplierIngredient
{
    public int SupplierIngredientId { get; private set; }

    public int SupplierId { get; private set; }
    public int IngredientId { get; private set; }

    public string Name { get; private set; } = null!;
    public decimal Price { get; private set; }
    public IngredientUnit Unit { get; private set; }
    public decimal Weight { get; private set; }
    public int ShelfLifeDays { get; private set; }
    public DateTime? DeletedAt { get; private set; }

    public Supplier Supplier { get; private set; } = null!;
    public Ingredient Ingredient { get; private set; } = null!;
    public ICollection<IngredientBatch> IngredientBatches { get; private set; } = new List<IngredientBatch>();

    public SupplierIngredient() { }

    public int QuantityBatches => IngredientBatches.Count;
    public decimal QuantityUnitInBatches => IngredientBatches.Sum(ib => ib.RemainingQuantity);

    public static SupplierIngredient Create(
        int supplierIngredientId,
        int supplierId,
        int ingredientId,
        string name,
        decimal price,
        IngredientUnit unit,
        decimal weight,
        int shelfLifeDays)
    {
        return new SupplierIngredient
        {
            SupplierIngredientId = supplierIngredientId,
            SupplierId = supplierId,
            IngredientId = ingredientId,
            Name = name,
            Price = price,
            Unit = unit,
            Weight = weight,
            ShelfLifeDays = shelfLifeDays
        };
    }

    public void Update(
        int supplierId,
        int ingredientId,
        string name,
        decimal price,
        IngredientUnit unit,
        decimal weight,
        int shelfLifeMonths)
    {
        SupplierId = supplierId;
        IngredientId = ingredientId;
        Name = name;
        Price = price;
        Unit = unit;
        Weight = weight;
        ShelfLifeDays = shelfLifeMonths;
    }

    public void SoftDelete()
    {
        DeletedAt = DateTime.UtcNow;
    }
}
