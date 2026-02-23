using GoldenBread.Domain.Entities;

namespace GoldenBread.Application.Features.ProductCatalog.Dtos;

public class ProductDetailResponse
{
    public int ProductId { get; private set; }
    public int CategoryId { get; private set; }

    public string Name { get; set; } = null!;
    public string Description { get; set; } = null!;
    public decimal SalePrice { get; set; }
    public decimal Weight { get; set; }
    public int ProductionTime { get; set; }

    public virtual ProductCategory Category { get; set; } = null!;

    public virtual ICollection<Favourite> Favourites { get; set; } = new List<Favourite>();
    public virtual ICollection<ProductImage> ProductImages { get; set; } = new List<ProductImage>();
}
