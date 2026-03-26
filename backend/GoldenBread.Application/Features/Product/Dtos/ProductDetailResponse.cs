namespace GoldenBread.Application.Features.Product.Dtos;

public record ProductDetailResponse
{
    public int ProductId { get; init; }
    public int CategoryId { get; init; }
    public string Name { get; init; } = null!;
    public string Description { get; init; } = null!;
    public decimal Weight { get; init; }
    public int ProductionTimeMinutes { get; init; }
    public int ShelfLifeDays { get; init; }
    public decimal StorageTempMin { get; init; }
    public decimal StorageTempMax { get; init; }

    // Категория
    public string CategoryName { get; init; } = null!;
    public string CategoryColor { get; init; } = null!;

    // Доступные партии
    public int CurrentBatchId { get; init; }
    public List<ProductBatchResponse> AvailableBatches { get; init; } = new();

    // Изображения
    public List<string> ImageUrls { get; init; } = new();

    // Флаги / Вычисляемые свойства
    public bool IsFavorite { get; init; }
    public int QuantityInCart { get; init; }
    public decimal TotalCostInCart { get; init; }

    // Рецепт
    public List<IngredientResponse> Ingredients { get; init; } = new();
}

public record ProductBatchResponse
{
    public int ProductBatchId { get; init; }
    public int QuantityPerBatch { get; init; }
    public decimal UnitPrice { get; init; }
    public decimal TotalPrice { get; init; }
}

public record IngredientResponse
{
    public int IngredientId { get; init; }
    public string Name { get; init; } = null!;
    public decimal Quantity { get; init; }
    public string Unit { get; init; } = null!;
}

public record ProductProjectionDto
{
    public ProductDetailResponse Detail { get; init; } = null!;
    public List<ProductBatchResponse> Batches { get; init; } = new();
    public List<IngredientResponse> Ingredients { get; init; } = new();
}
