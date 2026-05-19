namespace GoldenBread.Domain.Entities;

public class ProductBatch
{
    public int ProductBatchId { get; private set; }

    public int ProductId { get; private set; }

    public int MarkupPercent { get; set; }
    public int QuantityUnits { get; set; }

    public ICollection<CartItem> CartItems { get; set; } = new List<CartItem>();
    public ICollection<OrderItem> OrderItems { get; set; } = new List<OrderItem>();

    public Product Product { get; set; } = null!;

    public ProductBatch() { }

    public decimal UnitPrice => Product.CostPrice * (1 + MarkupPercent / 100m);
    public decimal TotalPrice => UnitPrice * QuantityUnits;

    public static ProductBatch Create(
    int productId,
    int quantityUnits,
    int markupPercent)
    {
        return new ProductBatch
        {
            ProductId = productId,
            QuantityUnits = quantityUnits,
            MarkupPercent = markupPercent
        };
    }

    public void Update(int markupPercent, int quantityUnits)
    {
        MarkupPercent = markupPercent;
        QuantityUnits = quantityUnits;
    }
}
