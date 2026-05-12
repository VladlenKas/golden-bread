
using GoldenBread.Desktop.Features.Common;

namespace GoldenBread.Desktop.Features.Procurement.PurchasePositions.Models;

public record class SupplierIngredientDto(
    int SupplierIngredientId,
    int SupplierId,
    int IngredientId,
    string Name,
    decimal Price,
    IngredientUnit Unit,
    decimal Weight,
    int ShelfLifeDays);

