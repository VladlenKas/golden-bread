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

    public ICollection<Favorite> Favorites { get; set; } = new List<Favorite>();
    public ICollection<ProductBatch> ProductBatches { get; set; } = new List<ProductBatch>();
    public ICollection<ProductImage> ProductImages { get; set; } = new List<ProductImage>();
    public ICollection<Recipe> Recipes { get; set; } = new List<Recipe>();

    public Product() { }

    #warning Добавить параметры для создания
    public static Product Create(
    string name,
    int productionTimeMinutes,
    decimal costPrice = 0)
    {
        return new Product
        {
            Name = name,
            ProductionTimeMinutes = productionTimeMinutes,
            CostPrice = costPrice,
            Description = string.Empty,
            Weight = 0,
            ShelfLifeDays = 0,
            StorageTempMin = 0,
            StorageTempMax = 0
        };
    }
}
