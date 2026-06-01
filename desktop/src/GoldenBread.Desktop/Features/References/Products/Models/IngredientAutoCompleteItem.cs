using GoldenBread.Desktop.Features.Common;

namespace GoldenBread.Desktop.Features.References.Products.Models;

public record IngredientAutoCompleteItem(int Id, string Name, IngredientUnit BaseUnit)
{
    public override string ToString() => Name;
}