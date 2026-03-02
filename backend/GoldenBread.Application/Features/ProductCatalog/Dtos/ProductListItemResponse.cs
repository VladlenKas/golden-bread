namespace GoldenBread.Application.Features.ProductCatalog.Dtos;

public class ProductListItemResponse
{
    public int ProductId { get; set; }
    public string Name { get; set; } = null!;
    public string Description { get; set; } = null!;
    public int ProductionTimeMinutes { get; set; }

    // Категория
    public int CategoryId { get; set; }
    public string CategoryName { get; set; } = null!;
    public string? CategoryColor { get; set; }

    // Минимальная партия (либо выбранная)
    public int ProductBatchId { get; set; }
    public int QuantityPerBatch { get; set; }  
    public decimal SalePrice { get; set; }

    // Первое изображение
    public string? ImageUrl { get; set; } 

    // Флаги / Вычисляемые свойства
    public bool IsFavorite { get; set; }
    public int QuantityInCart { get; set; }
}
    