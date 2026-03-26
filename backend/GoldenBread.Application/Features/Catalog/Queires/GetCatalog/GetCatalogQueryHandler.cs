using GoldenBread.Application.Abstractions.Data.Services;
using GoldenBread.Application.Abstractions.Services;
using GoldenBread.Application.Features.ProductCatalog.Dtos;

namespace GoldenBread.Application.Features.Catalog.Queires.GetCatalog;

public sealed class GetCatalogQueryHandler(
    ICatalogQueryService catalogQuery,
    ICurrentAccountContext accountContext) :
    IRequestHandler<GetCatalogQuery, CatalogResponse>
{
    public async Task<CatalogResponse> Handle(
        GetCatalogQuery query,
        CancellationToken ct)
    {
        int? companyId = await accountContext.GetCompanyIdAsync(ct);

        var data = await catalogQuery.GetCatalogAsync(companyId, ct);

        var productDtos = data.Products
            .Select(p => GetCatalogMapper.MapToResponse(p, companyId))
            .ToList();

        var categoryDtos = data.Categories
            .Select(c => new ProductCategoryResponse(
                c.ProductCategoryId,
                c.Name,
                c.Color,
                c.ProductsCount))
            .ToList();

        return new CatalogResponse(productDtos, categoryDtos);
    }
}

        