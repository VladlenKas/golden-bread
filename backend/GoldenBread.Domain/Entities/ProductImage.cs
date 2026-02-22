namespace GoldenBread.Domain.Entities;

public class ProductImage
{
    public int ProductImageId { get; set; }

    public int ProductId { get; set; }

    public byte[] Image { get; set; } = null!;

    public Product Product { get; set; } = null!;
}
