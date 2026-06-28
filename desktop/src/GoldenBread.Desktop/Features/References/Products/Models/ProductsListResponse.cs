using Avalonia.Media.Imaging;
using ReactiveUI;
using ReactiveUI.SourceGenerators;

namespace GoldenBread.Desktop.Features.References.Products.Models;

public partial class ProductListItem : ReactiveObject
{
    public int ProductId { get; set; }
    public string Name { get; set; } = string.Empty;
    public string Description { get; set; } = string.Empty;
    public int ProductionTimeMinutes { get; set; }
    public int CategoryId { get; set; }
    public string CategoryName { get; set; } = string.Empty;
    public string CategoryColor { get; set; } = string.Empty;
    public int ProductBatchId { get; set; }
    public int QuantityPerBatch { get; set; }
    public decimal SalePrice { get; set; }
    public string? ImageUrl { get; set; }
    public DateTime CreatedAt { get; set; }

    public int TotalSoldAllTime { get; set; }
    public List<SeasonalSalesData> SeasonalSales { get; set; } = new();
    public string? TopSeasonBadge { get; set; }

    public string SearchText => $"{Name}{Description}{CategoryName}".ToLowerInvariant();
    public decimal AmountPrice => SalePrice * QuantityPerBatch;

    [Reactive] private Bitmap? _productImage;
}

public record ProductsListResponse(List<ProductListItem> ProductsList);

public record SeasonalSalesData(
    Season Season,
    int Year,
    int TotalUnitsSold);