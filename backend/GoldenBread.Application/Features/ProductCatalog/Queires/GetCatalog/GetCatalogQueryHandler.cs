using GoldenBread.Application.Abstractions.Data;
using GoldenBread.Application.Features.ProductCatalog.Dtos;
using GoldenBread.Application.Services;

namespace GoldenBread.Application.Features.ProductCatalog.Queires.GetCatalog;

public sealed class GetCatalogQueryHandler(
    IGoldenBreadContext context,
    ICurrentAccountContext accountContext,
    IMapper mapper) :
    IRequestHandler<GetCatalogQuery, CatalogResponse>
{
    public async Task<CatalogResponse> Handle(
        GetCatalogQuery query, 
        CancellationToken cancellationToken)
    {
        int companyId = 0;
        var session = accountContext.GetSessionToken();

        if (!string.IsNullOrEmpty(session))
        {
            var account = await accountContext.GetAccountAsync(cancellationToken);
            companyId = account.Company.CompanyId;
        }

        var products = await context.Products
            .Include(p => p.Favourites)
            .Include(p => p.Category)
            .Include(p => p.ProductImages)
            .Include(p => p.ProductBatches)
                .ThenInclude(p => p.CartItems)
            .ToListAsync(cancellationToken);

        var categories = await context.ProductCategories
            .ToListAsync(cancellationToken);

        var productsList = mapper.Map<List<ProductListItemResponse>>(products,
            opts => opts.Items["CompanyId"] = companyId);

        var categoriesList = mapper.Map<List<ProductCategoryResponse>>(categories);

        return new CatalogResponse
        {
            ProductsList = productsList,
            Categories = categoriesList
        };
    }
}
        