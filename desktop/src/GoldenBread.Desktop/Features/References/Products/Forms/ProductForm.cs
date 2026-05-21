using GoldenBread.Desktop.Features.References.Products.Models;
using GoldenBread.Desktop.Infrastructure.Constants;
using GoldenBread.Desktop.UI.Common;
using ReactiveUI.SourceGenerators;
using System.ComponentModel.DataAnnotations;
using static GoldenBread.Desktop.UI.Common.PageViewModel;

namespace GoldenBread.Desktop.Features.References.Products.Forms;

public partial class ProductForm : ViewModelBase
{
    [Reactive]
    int _productId;

    [Reactive]
    [Required(ErrorMessage = ConstantMessages.RequiredValidation)]
    [StringLength(100, MinimumLength = 2, ErrorMessage = ConstantMessages.ProductNameLengthValidation)]
    [RegularExpression(ConstantRegularExpressions.Name, ErrorMessage = ConstantMessages.NameFormatValidation)]
    string _name = null!;

    [Reactive]
    [Required(ErrorMessage = ConstantMessages.RequiredValidation)]
    [StringLength(100, ErrorMessage = ConstantMessages.DescriptionLengthValidation)]
    [RegularExpression(ConstantRegularExpressions.Details, ErrorMessage = ConstantMessages.DetailsValidation)]
    string _description = null!;

    [Reactive]
    [Required(ErrorMessage = ConstantMessages.RequiredValidation)]
    [Range(0.01, 999999.99, ErrorMessage = ConstantMessages.CostPriceRangeValidation)]
    decimal _costPrice;

    [Reactive]
    [Required(ErrorMessage = ConstantMessages.RequiredValidation)]
    [Range(0.01, 999999.99, ErrorMessage = ConstantMessages.WeightRangeValidation)]
    decimal _weight;

    [Reactive]
    [Required(ErrorMessage = ConstantMessages.RequiredValidation)]
    [Range(1, 1440, ErrorMessage = ConstantMessages.ProductionTimeRangeValidation)]
    int _productionTimeMinutes;

    [Reactive]
    [Required(ErrorMessage = ConstantMessages.RequiredValidation)]
    [Range(1, 60, ErrorMessage = ConstantMessages.ProductShelfLifeRangeValidation)]
    int _shelfLifeDays;

    [Reactive]
    [Required(ErrorMessage = ConstantMessages.RequiredValidation)]
    [Range(-50, 100, ErrorMessage = ConstantMessages.StorageTempRangeValidation)]
    decimal _storageTempMin;

    [Reactive]
    [Required(ErrorMessage = ConstantMessages.RequiredValidation)]
    [Range(-50, 100, ErrorMessage = ConstantMessages.StorageTempRangeValidation)]
    decimal _storageTempMax;

    [Reactive]
    [Required(ErrorMessage = ConstantMessages.CategoryRequiredValidation)]
    [Range(1, int.MaxValue, ErrorMessage = ConstantMessages.CategoryRequiredValidation)]
    int _categoryId;

    [Reactive]
    ItemsAutoCompleteBox? _selectedCategoryItem;

    public static ProductForm FromDto(ProductDto dto) => new()
    {
        ProductId = dto.ProductId,
        Name = dto.Name,
        Description = dto.Description,
        CostPrice = dto.CostPrice,
        Weight = dto.Weight,
        ProductionTimeMinutes = dto.ProductionTimeMinutes,
        ShelfLifeDays = dto.ShelfLifeDays,
        StorageTempMin = dto.StorageTempMin,
        StorageTempMax = dto.StorageTempMax,
        CategoryId = dto.CategoryId
    };

    public ProductDto ToDto() => new(
        ProductId, Name, Description, CostPrice, Weight,
        ProductionTimeMinutes, ShelfLifeDays,
        StorageTempMin, StorageTempMax, CategoryId);

    public ProductForm Clone() => new()
    {
        ProductId = ProductId,
        Name = Name,
        Description = Description,
        CostPrice = CostPrice,
        Weight = Weight,
        ProductionTimeMinutes = ProductionTimeMinutes,
        ShelfLifeDays = ShelfLifeDays,
        StorageTempMin = StorageTempMin,
        StorageTempMax = StorageTempMax,
        CategoryId = CategoryId,
        SelectedCategoryItem = SelectedCategoryItem
    };

    public bool EqualsValues(ProductForm? other)
    {
        if (other is null) return false;
        return ProductId == other.ProductId && Name == other.Name
            && Description == other.Description && CostPrice == other.CostPrice
            && Weight == other.Weight && ProductionTimeMinutes == other.ProductionTimeMinutes
            && ShelfLifeDays == other.ShelfLifeDays
            && StorageTempMin == other.StorageTempMin
            && StorageTempMax == other.StorageTempMax
            && CategoryId == other.CategoryId;
    }
}