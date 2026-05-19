namespace GoldenBread.Application.Features.Products.Dtos;

public record ProductListItem(
    int ProductId,
    string Name,
    string Description,
    int ProductionTimeMinutes,
    int CategoryId,
    string CategoryName,
    string CategoryColor,
    int ProductBatchId,
    int QuantityPerBatch,
    decimal SalePrice,
    string? ImageUrl);

public record ProductsListResponse(List<ProductListItem> ProductsList);
