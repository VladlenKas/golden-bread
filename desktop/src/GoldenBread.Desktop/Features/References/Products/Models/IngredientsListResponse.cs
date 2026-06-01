using GoldenBread.Desktop.Features.Common;
using GoldenBread.Desktop.UI.Helpers;
using System.Reactive;

namespace GoldenBread.Desktop.Features.References.Products.Models;

public record IngredientsListResponse(List<IngredientListItem> IngredientsList);

public record IngredientListItem(
    int IngredientId,
    string Name,
    IngredientUnit BaseUnit,
    int RecipesCount,
    int SupplierIngredientsCount)
{
    public string SearchText = $"{Name}{BaseUnit}".ToLowerInvariant();
    public string UnitFormatted => LocalizedIngredientUnits.UnitsTable(BaseUnit);
};