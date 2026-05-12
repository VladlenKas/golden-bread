using GoldenBread.Domain.Enums;

namespace GoldenBread.Application.Features.SupplierIngredient.Dtos;

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
    decimal QuantityUnitInBatches);