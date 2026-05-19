namespace GoldenBread.Application.Features.ProductCategory.Dtos;

public record ProductCategoryListItem(
    int ProductCategoryId,
    string Name,
    string Color,
    int ProductsCount);

public record ProductCategoriesListResponse(List<ProductCategoryListItem> CategoriesList);
