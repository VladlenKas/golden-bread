namespace GoldenBread.Application.Features.ProductCatalog.Dtos;

public class ProductDetailResponse
{
    public int ProductId { get; set; }
    public int CategoryId { get; set; }
    public string Name { get; set; } = null!;
    public string Description { get; set; } = null!;
    public decimal Weight { get; set; }
    public int ProductionTimeMinutes { get; set; }
    public int ShelfLifeDays { get; set; }
    public decimal StorageTempMin { get; set; }
    public decimal StorageTempMax { get; set; }

    // Категория
    public string CategoryName { get; set; } = null!;
    public string CategoryColor { get; set; } = null!;
        
    // Доступные партии
    public int CurrentBatchId { get; set; }
    public List<ProductBatchResponse> AvailableBatches { get; set; } = new();

    // Изображения
    public List<string> ImageUrls { get; set; } = new();

    // Флаги / Вычисляемые свойства
    public bool IsFavorite { get; set; }
    public int QuantityInCart { get; set; }     
    public decimal TotalCostInCart { get; set; }

    // Рецепт
    public List<IngredientResponse> Ingredients { get; set; } = new();
}

public class ProductBatchResponse
{
    public int ProductBatchId { get; set; }
    public int QuantityPerBatch { get; set; }
    public decimal UnitPrice { get; set; }
    public decimal TotalPrice { get; set; }
}

public class IngredientResponse
{
    public int IngredientId { get; set; }
    public string Name { get; set; } = null!;
    public decimal Quantity { get; set; }
    public string Unit { get; set; } = null!;
}