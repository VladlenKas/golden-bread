using System.Collections.Generic;

namespace GoldenBread.Domain.Entities;

public class ProductBatch
{
    public int ProductBatchId { get; set; }

    public int ProductId { get; set; }

    public int QuantityPerBatch { get; set; }

    public ICollection<CartItem> CartItems { get; set; } = new List<CartItem>();
    public ICollection<OrderItem> OrderItems { get; set; } = new List<OrderItem>();

    public Product Product { get; set; } = null!;
}
