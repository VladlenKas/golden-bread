using GoldenBread.Desktop.Infrastructure.Constants;
using GoldenBread.Desktop.UI.Common;
using System.ComponentModel.DataAnnotations;
using ReactiveUI.SourceGenerators;
using GoldenBread.Desktop.Features.Common;
using GoldenBread.Desktop.Features.References.Products.Models;

namespace GoldenBread.Desktop.Features.Procurement.PurchasePositions.Models;

public partial class IngredientForm : ViewModelBase
{
    [Reactive]
    int _ingredientId;

    [Reactive]
    [Required(ErrorMessage = ConstantMessages.RequiredValidation)]
    [StringLength(100, MinimumLength = 2, ErrorMessage = ConstantMessages.NameLengthValidation)]
    string _name = null!;

    [Reactive]
    [Required(ErrorMessage = ConstantMessages.RequiredValidation)]
    IngredientUnit _baseUnit;

    public static IngredientForm FromDto(IngredientDto dto)
    {
        return new IngredientForm
        {
            IngredientId = dto.IngredientId,
            Name = dto.Name,
            BaseUnit = dto.BaseUnit
        };
    }

    public IngredientDto ToDto()
    {
        return new IngredientDto(IngredientId, Name, BaseUnit);
    }

    public IngredientForm Clone() => new()
    {
        IngredientId = IngredientId,
        Name = Name,
        BaseUnit = BaseUnit
    };

    public bool EqualsValues(IngredientForm? other)
    {
        if (other is null) return false;
        return IngredientId == other.IngredientId
            && Name == other.Name
            && BaseUnit == other.BaseUnit;
    }
}