using GoldenBread.Application.Abstractions.Data;
using GoldenBread.Application.Features.ProductCategory.Dtos;

namespace GoldenBread.Application.Features.ProductCategory.Queries.GetProductCategoriesList;

public sealed class GetProductCategoriesListQueryHandler(IGoldenBreadContext context)
    : IRequestHandler<GetProductCategoriesListQuery, ProductCategoriesListResponse>
{
    public async Task<ProductCategoriesListResponse> Handle(GetProductCategoriesListQuery query, CancellationToken ct)
    {
        var list = await context.ProductCategories
            .Where(c => c.DeletedAt == null)
            .Select(c => new ProductCategoryListItem(
                c.ProductCategoryId,
                c.Name,
                c.Color,
                c.Products.Count(p => p.DeletedAt == null)))
            .ToListAsync(ct);

        return new ProductCategoriesListResponse(list);
    }
}