using GoldenBread.Application.Abstractions.Data;
using GoldenBread.Application.Features.ProductCatalog.Dtos;
using GoldenBread.Application.Services;

namespace GoldenBread.Application.Features.ProductCatalog.Queires.GetProductsList;

public sealed class GetProductsListQueryHandler(
    IGoldenBreadContext context,
    ICurrentAccountContext accountContext,
    IMapper mapper) :
    IRequestHandler<GetProductsListQuery, List<ProductListItemResponse>>
{
    public async Task<List<ProductListItemResponse>> Handle(
        GetProductsListQuery query, 
        CancellationToken cancellationToken)
    {
        int companyId = 0;
        var session = accountContext.GetSessionFromCookie();

        if (!string.IsNullOrEmpty(session))
        {
            var account = await accountContext.GetAccountAsync(cancellationToken);
            companyId = (await context.Companies
                .FirstOrDefaultAsync(c => 
                    c.AccountId == account.AccountId,
                    cancellationToken))?
                .CompanyId ?? 0;
        }
            
        var products = await context.Products
            .AsSplitQuery()
            .Include(p => p.Favourites)
            .Include(p => p.Category)
            .Include(p => p.ProductImages)  
            .Include(p => p.ProductBatches)
                .ThenInclude(p => p.CartItems)
            .ToListAsync(cancellationToken);

        return mapper.Map<List<ProductListItemResponse>>(products,
            opts => opts.Items["CompanyId"] = companyId);
    }
}
