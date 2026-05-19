using GoldenBread.Desktop.Features.References.Products.Models;
using GoldenBread.Desktop.UI.Common;
using ReactiveUI;
using ReactiveUI.SourceGenerators;
using System.ComponentModel.DataAnnotations;
using System.Text.RegularExpressions;

namespace GoldenBread.Desktop.Features.References.Products.Forms;

public partial class ProductBatchForm : ViewModelBase
{
    public ProductBatchForm()
    {
        this.WhenAnyValue(
            x => x.BaseUnitPrice,
            x => x.MarkupPercent,
            x => x.QuantityUnits)
            .Subscribe(_ =>
            {
                if (QuantityUnits <= 0 || BaseUnitPrice <= 0)
                {
                    FinalUnitPrice = 0;
                    BatchTotalPrice = 0;
                }
                else
                {
                    // Цена за 1 шт с наценкой
                    FinalUnitPrice = BaseUnitPrice * (1 + MarkupPercent / 100m);
                    // Общая сумма партии
                    BatchTotalPrice = FinalUnitPrice * QuantityUnits;
                }
            });
    }

    [Reactive] 
    int? _productBatchId;

    [Reactive]
    [Required]
    [Range(0, 1000)] 
    int _markupPercent;

    [Reactive]
    [Required]
    [Range(1, 25)] 
    int _quantityUnits;

    // Базовая цена за 1 шт от продукта (приходит извне, не вычисляется)
    [Reactive]
    decimal _baseUnitPrice;

    // Общая цена всей партии (ранее CostPrice)
    [Reactive]
    decimal _batchTotalPrice;

    // Итоговая цена за 1 шт с наценкой (ранее UnitPriceInBatch)
    [Reactive]
    decimal _finalUnitPrice;

    public static ProductBatchForm FromDto(ProductBatchDto dto) => new()
    {
        ProductBatchId = dto.ProductBatchId,
        MarkupPercent = dto.MarkupPercent,
        QuantityUnits = dto.QuantityUnits
    };

    public ProductBatchDto ToDto(int productId) => new(
        ProductBatchId, productId, MarkupPercent, QuantityUnits);

    public ProductBatchForm Clone() => new()
    {
        ProductBatchId = ProductBatchId,
        MarkupPercent = MarkupPercent,
        QuantityUnits = QuantityUnits,
        BaseUnitPrice = BaseUnitPrice
    };

    public bool EqualsValues(ProductBatchForm? other)
    {
        if (other is null) return false;
        return ProductBatchId == other.ProductBatchId
            && MarkupPercent == other.MarkupPercent
            && QuantityUnits == other.QuantityUnits;
    }
}
