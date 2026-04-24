namespace GoldenBread.Domain.Entities;

public class ProductCategory
{
    public int ProductCategoryId { get; set; }

    public string Name { get; set; } = null!;
    public string Color { get; set; } = null!;
    public DateTime? DeletedAt { get; set; }

    public ICollection<Product> Products { get; set; } = new List<Product>();
}
