namespace GoldenBread.Domain.Entities;

public class CartItem
{
    public int CartItemId { get; private set; }

    public int CompanyId { get; private set; }
    public int BatchId { get; private set; }

    public int Quantity { get; set; }

    public ProductBatch Batch { get; set; } = null!;
    public Company Company { get; set; } = null!;

    public decimal TotalCost => Batch.TotalPrice * Quantity;

    public CartItem() { }

    public static CartItem Create(
        Company company,
        ProductBatch batch,
        int quantity)
    {
        return new CartItem()
        {
            Company = company,
            Batch = batch,
            Quantity = quantity
        };
    }

    public void Update(
        ProductBatch batch,
        int quantity)
    {
        Batch = batch;
        Quantity = quantity;
    }
}
