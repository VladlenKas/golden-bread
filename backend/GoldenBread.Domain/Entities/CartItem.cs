namespace GoldenBread.Domain.Entities;

public class CartItem
{
    public int CartItemId { get; set; }

    public int CompanyId { get; set; }
    public int? BatchId { get; set; }

    public int Quantity { get; set; }

    public ProductBatch? Batch { get; set; }
    public Company Company { get; set; } = null!;
}
