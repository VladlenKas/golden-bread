using GoldenBread.Application.Abstractions.Data.Services;
using GoldenBread.Application.Abstractions.Services;
using GoldenBread.Application.Features.Catalog.Dtos;
using GoldenBread.Application.Features.Catalog.Mapping;

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
        int? companyId = null;
        try
        {
            companyId = await accountContext.GetCompanyIdAsync(ct);
        }
        catch { }

        var data = await catalogQuery.GetCatalogAsync(companyId, ct);

        var productDtos = data.Products
            .Select(p =>
            {
                var stats = data.SalesStatistics.GetValueOrDefault(p.ProductId);
                return CatalogMapper.ToListItems(p, companyId, stats);
            })
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