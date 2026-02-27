namespace GoldenBread.Application.Features.ProductCatalog.Dtos;

public class CatalogResponse
{
    public List<ProductListItemResponse> ProductsList { get; set; } = null!;
    public List<ProductCategoryResponse> Categories { get; set; } = null!; 
}

public class ProductCategoryResponse
{
    public int ProductCategoryId { get; set; }

    public string Name { get; set; } = null!;
    public string Color { get; set; } = null!;

    public int ProductCount { get; set; }
}
