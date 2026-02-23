using GoldenBread.Application.Abstractions.Data;
using GoldenBread.Application.Features.ProductCatalog.Dtos;

namespace GoldenBread.Application.Features.ProductCatalog.Queires.GetProductsList;

public sealed class GetProductsListQueryHandler(
    IGoldenBreadContext context,
    IMapper mapper) :
    IRequestHandler<GetProductsListQuery, List<ProductListItemResponse>>
{
    public async Task<List<ProductListItemResponse>> Handle(
        GetProductsListQuery query, 
        CancellationToken cancellationToken)
    {
        var products = await context.Products
            .AsNoTracking()
            .Include(p => p.Category)
            .Include(p => p.ProductImages)
            .Include(p => p.ProductBatches)
            .ToListAsync(cancellationToken);

        return mapper.Map<List<ProductListItemResponse>>(products);
    }
}
