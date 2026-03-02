namespace GoldenBread.Domain.Entities;

public class Product
{
    public int ProductId { get; private set; }

    public int CategoryId { get; private set; }

    public string Name { get; set; } = null!;
    public string Description { get; set; } = null!;
    public decimal CostPrice { get; set; }
    public decimal Weight { get; set; }
    public int ProductionTimeMinutes { get; set; }
    public int ShelfLifeDays { get; set; }
    public decimal StorageTempMin { get; set; }
    public decimal StorageTempMax { get; set; }
    public DateTime? DeletedAt { get; set; }

    public ProductCategory Category { get; set; } = null!;

    public ICollection<Favorite> Favourites { get; set; } = new List<Favorite>();
    public ICollection<ProductBatch> ProductBatches { get; set; } = new List<ProductBatch>();
    public ICollection<ProductImage> ProductImages { get; set; } = new List<ProductImage>();
    public ICollection<Recipe> Recipes { get; set; } = new List<Recipe>();

    public Product() { }
}
