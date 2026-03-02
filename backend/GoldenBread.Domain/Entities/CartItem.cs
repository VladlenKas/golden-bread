namespace GoldenBread.Domain.Entities;

public class CartItem
{
    public int CartItemId { get; private set; }

    public int CompanyId { get; private set; }
    public int BatchId { get; private set; }

    public int Quantity { get; set; }

    public ProductBatch Batch { get; set; } = null!;
    public Company Company { get; set; } = null!;

    public decimal TotalPrice => Batch.TotalPrice * Quantity;

    public CartItem() { }

    public static CartItem Create(
        int companyId,
        int batchId,
        int quantity)
    {
        return new CartItem()
        {
            CompanyId = companyId,
            BatchId = batchId,
            Quantity = quantity
        };
    }

    public void Update(
        int batchId,
        int quantity)
    {
        BatchId = batchId;
        Quantity = quantity;
    }
}
