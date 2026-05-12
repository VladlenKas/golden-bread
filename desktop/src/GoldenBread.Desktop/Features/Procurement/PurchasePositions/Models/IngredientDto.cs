using GoldenBread.Desktop.Features.Common;

namespace GoldenBread.Desktop.Features.Procurement.PurchasePositions.Models;

public record class IngredientDto(
    int IngredientId,
    string Name,
    IngredientUnit BaseUnit);