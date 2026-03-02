namespace GoldenBread.Domain.Entities;

public class ProductBatch
{
    public int ProductBatchId { get; private set; }

    public int ProductId { get; private set; }

    public int MarkupPercent { get; set; }
    public int QuantityPerBatch { get; set; }

    public ICollection<CartItem> CartItems { get; set; } = new List<CartItem>();
    public ICollection<OrderItem> OrderItems { get; set; } = new List<OrderItem>();

    public Product Product { get; set; } = null!;

    public ProductBatch() { }

    public decimal UnitPrice => Product.CostPrice * (1 + MarkupPercent / 100m);
    public decimal TotalPrice => UnitPrice * QuantityPerBatch;
}
