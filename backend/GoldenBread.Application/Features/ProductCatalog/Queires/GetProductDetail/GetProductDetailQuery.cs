using GoldenBread.Application.Features.ProductCatalog.Dtos;

namespace GoldenBread.Application.Features.ProductCatalog.Queires.GetProductDetail;

public sealed record class GetProductDetailQuery :
    IRequest<ProductDetailResponse>;
