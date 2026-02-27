namespace GoldenBread.Application.Features.ProductCatalog.Dtos;

public class ProductDetailResponse
{
    public int ProductId { get; set; }
    public int CategoryId { get; set; }

    public string Name { get; set; } = null!;
    public string Description { get; set; } = null!;
    public decimal SalePrice { get; set; }
    public decimal Weight { get; set; }
    public int ProductionTime { get; set; }

    // Категория
    public string CategoryName { get; set; } = null!;
    public string CategoryColor { get; set; } = null!;
        
    // Поставки
    public int ProductBatchId { get; set; }
    public int QuantityPerBatch { get; set; }
    public List<ProductBatchResponse> AvailableBatches { get; set; } = new();

    // Медиа
    public List<string> ImageUrls { get; set; } = new();

    // Пользовательские данные
    public bool IsFavorite { get; set; }
    public int QuantityInCart { get; set; }

    // Рецепт
    public List<IngredientResponse> Ingredients { get; set; } = new();
}

public class ProductBatchResponse
{
    public int ProductBatchId { get; set; }
    public int QuantityPerBatch { get; set; }
    public decimal SalePrice { get; set; }
}

public class IngredientResponse
{
    public int IngredientId { get; set; }
    public string Name { get; set; } = null!;
    public decimal Quantity { get; set; }
    public string Unit { get; set; } = null!;
}