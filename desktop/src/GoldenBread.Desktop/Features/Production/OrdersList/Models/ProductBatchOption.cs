namespace GoldenBread.Desktop.Features.Production.OrdersList.Models;

public class ProductBatchOption
{
    public int ProductBatchId { get; set; }
    public int MarkupPercent { get; set; }
    public int QuantityUnits { get; set; }
    public decimal CostPrice { get; set; }

    public decimal UnitPrice => CostPrice * (1 + MarkupPercent / 100m);
    public decimal BatchPrice => UnitPrice * QuantityUnits;
    public string DisplayText => $"{QuantityUnits} шт. × {UnitPrice:N2} ₽ = {BatchPrice:N2} ₽";
}