using GoldenBread.Desktop.Features.Common;
using GoldenBread.Desktop.Infrastructure.Constants;
using GoldenBread.Desktop.UI.Common;
using ReactiveUI.SourceGenerators;
using System.ComponentModel.DataAnnotations;

namespace GoldenBread.Desktop.Features.Procurement.PurchasePositions.Models;

public partial class SupplierIngredientForm : ViewModelBase
{
    [Reactive]
    int _supplierIngredientId;

    [Reactive]
    [Required(ErrorMessage = ConstantMessages.SupplierRequiredValidation)]
    int _supplierId;

    [Reactive]
    [Required(ErrorMessage = ConstantMessages.IngredientRequiredValidation)]
    int _ingredientId;

    [Reactive]
    [Required(ErrorMessage = ConstantMessages.RequiredValidation)]
    [StringLength(100, MinimumLength = 2, ErrorMessage = ConstantMessages.NameLengthValidation)]
    string _name = null!;

    [Reactive]
    [Required(ErrorMessage = ConstantMessages.RequiredValidation)]
    [Range(0.01, 999999.99, ErrorMessage = ConstantMessages.PriceRangeValidation)]
    decimal _price;

    [Reactive]
    [Required(ErrorMessage = ConstantMessages.RequiredUnitValidation)]
    IngredientUnit _unit;

    [Reactive]
    [Required(ErrorMessage = ConstantMessages.RequiredValidation)]
    [Range(0.01, 999999.99, ErrorMessage = ConstantMessages.WeightRangeValidation)]
    decimal _weight;

    [Reactive]
    [Required(ErrorMessage = ConstantMessages.RequiredValidation)]
    [Range(1, 1200, ErrorMessage = ConstantMessages.ShelfLifeRangeValidation)]
    int _shelfLifeDays;

    public static SupplierIngredientForm FromDto(SupplierIngredientDto dto)
    {
        return new SupplierIngredientForm
        {
            SupplierIngredientId = dto.SupplierIngredientId,
            SupplierId = dto.SupplierId,
            IngredientId = dto.IngredientId,
            Name = dto.Name,
            Price = dto.Price,
            Unit = dto.Unit,
            Weight = dto.Weight,
            ShelfLifeDays = dto.ShelfLifeDays
        };
    }

    public SupplierIngredientDto ToDto()
    {
        return new SupplierIngredientDto(
            SupplierIngredientId,
            SupplierId,
            IngredientId,
            Name,
            Price,
            Unit,
            Weight,
            ShelfLifeDays);
    }

    public SupplierIngredientForm Clone() => new()
    {
        SupplierIngredientId = SupplierIngredientId,
        SupplierId = SupplierId,
        IngredientId = IngredientId,
        Name = Name,
        Price = Price,
        Unit = Unit,
        Weight = Weight,
        ShelfLifeDays = ShelfLifeDays
    };

    public bool EqualsValues(SupplierIngredientForm? other)
    {
        if (other is null) return false;
        return SupplierIngredientId == other.SupplierIngredientId
            && SupplierId == other.SupplierId
            && IngredientId == other.IngredientId
            && Name == other.Name
            && Price == other.Price
            && Unit == other.Unit
            && Weight == other.Weight
            && ShelfLifeDays == other.ShelfLifeDays;
    }
}