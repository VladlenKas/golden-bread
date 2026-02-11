namespace GoldenBread.Domain.Entities;

public partial class CartItem
{
    public int CartItemId { get; set; }
    public int AccountId { get; set; }
    public int? BatchId { get; set; }
    public int Quantity { get; set; }
    public virtual ProductBatch? Batch { get; set; }
    public virtual Account Account { get; set; } = null!;
}
