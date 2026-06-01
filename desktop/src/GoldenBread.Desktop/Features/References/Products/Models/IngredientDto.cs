using GoldenBread.Desktop.Features.Common;

namespace GoldenBread.Desktop.Features.References.Products.Models;

public record class IngredientDto(
    int IngredientId,
    string Name,
    IngredientUnit BaseUnit);