namespace GoldenBread.Application.Features.ProductCatalog.Dtos;

public sealed record ProductListItemResponse(
    // Основные данные
    int ProductId,
    string Name,
    string Description,
    int ProductionTimeMinutes,

    // Категория
    int CategoryId,
    string CategoryName,
    string? CategoryColor,

    // Минимальная партия (либо выбранная)
    int ProductBatchId,
    int QuantityPerBatch,

    // Доп. данные
    decimal SalePrice,
    string? ImageUrl,
    bool IsFavorite,
    int QuantityInCart);

public sealed record ProductCategoryResponse(
    int ProductCategoryId,
    string Name,
    string Color,
    int ProductsCount);

public sealed record CatalogResponse(
    List<ProductListItemResponse> ProductsList,
    List<ProductCategoryResponse> Categories);