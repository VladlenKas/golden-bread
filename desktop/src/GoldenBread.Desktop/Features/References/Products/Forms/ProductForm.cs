using GoldenBread.Desktop.Features.References.Products.Models;
using GoldenBread.Desktop.UI.Common;
using System.ComponentModel.DataAnnotations;
using ReactiveUI.SourceGenerators;

namespace GoldenBread.Desktop.Features.References.Products.Forms;

public partial class ProductForm : ViewModelBase
{
    [Reactive] int _productId;
    [Reactive][Required][StringLength(100, MinimumLength = 2)] string _name = null!;
    [Reactive][Required][StringLength(500)] string _description = null!;
    [Reactive][Required][Range(0.01, 999999.99)] decimal _costPrice;
    [Reactive][Required][Range(0.01, 999999.99)] decimal _weight;
    [Reactive][Required][Range(1, 1440)] int _productionTimeMinutes;
    [Reactive][Required][Range(0, 3650)] int _shelfLifeDays;
    [Reactive][Required][Range(-50, 100)] decimal _storageTempMin;
    [Reactive][Required][Range(-50, 100)] decimal _storageTempMax;
    [Reactive][Required][Range(1, int.MaxValue)] int _categoryId;

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
        CategoryId = CategoryId
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