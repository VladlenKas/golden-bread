using GoldenBread.Desktop.UI.Helpers;

namespace GoldenBread.Desktop.Features.References.Products.Models;

public record ProductDetailResponse(
    int ProductId,
    string Name,
    string Description,
    decimal CostPrice,
    decimal Weight,
    int ProductionTimeMinutes,
    int ShelfLifeDays,
    decimal StorageTempMin,
    decimal StorageTempMax,
    int CategoryId,
    string CategoryName,
    string CategoryColor,
    int CurrentBatchId,
    List<ProductBatchResponse> AvailableBatches,
    List<string> ImageUrls,
    List<IngredientResponse> Ingredients);

public record ProductBatchResponse(
    int ProductBatchId,
    int QuantityPerBatch,
    int MarkupPercent,
    decimal UnitPrice,
    decimal TotalPrice);

public record IngredientResponse(
    int IngredientId,
    string Name,
    decimal Quantity,
    string Unit)
{
    public string UnitLocalized => LocalizedIngredientUnits.UnitsDetail(Unit);
};