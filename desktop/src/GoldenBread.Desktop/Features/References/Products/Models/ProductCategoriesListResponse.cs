namespace GoldenBread.Desktop.Features.References.Products.Models;

public record ProductCategoryListItem(
    int ProductCategoryId, 
    string Name, 
    string Color, 
    int ProductsCount)
{
    public string SearchText = $"{Name}".ToLowerInvariant();
};
public record ProductCategoriesListResponse(List<ProductCategoryListItem> CategoriesList);