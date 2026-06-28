using System.ComponentModel.DataAnnotations;
using GoldenBread.Desktop.Features.References.Products.Models;
using GoldenBread.Desktop.Infrastructure.Constants;
using GoldenBread.Desktop.UI.Common;
using ReactiveUI.SourceGenerators;

namespace GoldenBread.Desktop.Features.References.Products.Forms;

public partial class ProductCategoryForm : ViewModelBase
{
    [Reactive] int _productCategoryId;

    [Reactive]
    [Required(ErrorMessage = ConstantMessages.RequiredValidation)]
    [StringLength(50, MinimumLength = 2)] string _name = null!;

    [Reactive]
    [Required(ErrorMessage = ConstantMessages.RequiredValidation)]
    [RegularExpression(@"^#[0-9A-Fa-f]{6}$", ErrorMessage = "Цвет должен соответствовать HEX значению")] string _color = "#FF0000";

    public static ProductCategoryForm FromDto(ProductCategoryDto dto) => new()
    {
        ProductCategoryId = dto.ProductCategoryId,
        Name = dto.Name,
        Color = dto.Color
    };

    public ProductCategoryDto ToDto() => new(
        ProductCategoryId, Name, Color);

    public ProductCategoryForm Clone() => new()
    {
        ProductCategoryId = ProductCategoryId,
        Name = Name,
        Color = Color
    };

    public bool EqualsValues(ProductCategoryForm? other)
    {
        if (other is null) return false;
        return ProductCategoryId == other.ProductCategoryId
            && Name == other.Name && Color == other.Color;
    }
}