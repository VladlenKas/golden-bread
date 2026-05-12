using GoldenBread.Desktop.Features.Common;
using GoldenBread.Desktop.UI.Helpers;

namespace GoldenBread.Desktop.Features.Procurement.PurchasePositions.Models;

public record SupplierIngredientsListResponse(List<SupplierIngredientListItem> SupplierIngredientsList);

public record SupplierIngredientListItem(
    int SupplierIngredientId,
    string Name,
    string SupplierName,
    string IngredientName,
    IngredientUnit Unit,
    decimal Weight,
    decimal Price,
    int ShelfLifeMonths,
    int QuantityBatches,
    decimal QuantityUnitInBatches)
{
    public string SearchText = $"{Name}{SupplierName}{IngredientName}{Unit}{Weight}{Price}".ToLowerInvariant();
    public string UnitFormatted => LocalizedIngredientUnits.UnitsTable(Unit);
    public string WeightFormatted => $"{Weight:F3} {LocalizedIngredientUnits.UnitsTable(Unit)}";
    public string PriceFormatted => $"{Price:C2}";
    public string ShelfLifeFormatted => $"{ShelfLifeMonths} дней";
    public string QuantityBatchesFormatted => $"{QuantityBatches} шт.";
    public string QuantityUnitInBatchesFormatted => $"{QuantityUnitInBatches:F3} {LocalizedIngredientUnits.UnitsTable(Unit)}";
};
