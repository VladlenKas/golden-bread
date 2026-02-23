namespace GoldenBread.Application.Features.ProductCatalog.Dtos;

public class ProductListItemResponse
{
    public int ProductId { get; set; }
    public string Name { get; set; } = null!;
    public string Description { get; set; } = null!;
    public int ProductionTime { get; set; }

    public int CategoryId { get; set; }
    public string CategoryName { get; set; } = null!;
    public string? CategoryColor { get; set; }

    public int QuantityPerBatch { get; set; }  
    public decimal SalePrice { get; set; }

    public string? ImageUrl { get; set; } // Изображение для превью
}
