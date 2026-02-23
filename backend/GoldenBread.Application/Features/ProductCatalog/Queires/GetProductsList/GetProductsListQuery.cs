using GoldenBread.Application.Features.ProductCatalog.Dtos;

namespace GoldenBread.Application.Features.ProductCatalog.Queires.GetProductsList;

public sealed record class GetProductsListQuery :
    IRequest<List<ProductListItemResponse>>;
