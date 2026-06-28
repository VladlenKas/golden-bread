using GoldenBread.Desktop.Features.References.Products.Models;
using GoldenBread.Desktop.Infrastructure.Constants;
using GoldenBread.Desktop.UI.Common;
using GoldenBread.Desktop.UI.Helpers;
using ReactiveUI.SourceGenerators;
using System.ComponentModel.DataAnnotations;

namespace GoldenBread.Desktop.Features.References.Products.Forms;

public partial class RecipeItemForm : ViewModelBase
{
    [Reactive] 
    int? _recipeId;

    [Reactive]
    [Required(ErrorMessage = ConstantMessages.RequiredValidation)]
    [Range(1, int.MaxValue, ErrorMessage = "Значение ID должно начинаться от 1")] int _ingredientId;

    [Reactive] 
    string _ingredientName = string.Empty;

    [Reactive]
    [Required(ErrorMessage = ConstantMessages.RequiredValidation)]
    [Range(0.01, 999999.99, ErrorMessage = "Значение количества должно начинаться от 0.01 до 999999.99")] decimal _quantity;

    public string Unit { get; set; } = string.Empty;
    public string UnitLocalized => LocalizedIngredientUnits.UnitsDetail(Unit);

    public static RecipeItemForm FromDto(RecipeItemDto dto) => new()
    {
        RecipeId = dto.RecipeId,
        IngredientId = dto.IngredientId,
        Quantity = dto.Quantity
    };

    public RecipeItemDto ToDto() => new(RecipeId, IngredientId, Quantity);

    public RecipeItemForm Clone() => new()
    {
        RecipeId = RecipeId,
        IngredientId = IngredientId,
        IngredientName = IngredientName,
        Quantity = Quantity,
        Unit = Unit
    };

    public bool EqualsValues(RecipeItemForm? other)
    {
        if (other is null) return false;
        return RecipeId == other.RecipeId
            && IngredientId == other.IngredientId
            && IngredientName == other.IngredientName
            && Quantity == other.Quantity;
    }
}
