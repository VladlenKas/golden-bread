namespace GoldenBread.Application.Features.Catalog.Dtos;

public sealed record ProductDetailResponse(
    // Основные данные
    int ProductId,
    int CategoryId,
    string Name,
    string Description,
    decimal Weight,
    decimal CostPrice,
    int ProductionTimeMinutes,
    int ShelfLifeDays,
    decimal StorageTempMin,
    decimal StorageTempMax,

    // Категория
    string CategoryName,
    string CategoryColor,

    // Доступные партии
    int CurrentBatchId,
    List<ProductBatchResponse> AvailableBatches,

    // Доп. данные
    List<string> ImageUrls,
    bool IsFavorite,
    int QuantityInCart,
    decimal TotalCostInCart,

    // Рецепт
    List<IngredientResponse> Ingredients);

public sealed record ProductBatchResponse(
    int ProductBatchId,
    int QuantityPerBatch,
    int MarkupPercent,
    decimal UnitPrice,
    decimal TotalPrice);

public record IngredientResponse(
  int IngredientId,
  string Name,
  decimal Quantity,
  string Unit
);