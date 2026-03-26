using GoldenBread.Application.Features.Catalog.Dtos;

namespace GoldenBread.Application.Features.Catalog.Queires.GetProductDetail;

public sealed record class GetProductDetailQuery(int ProductId) :
    IRequest<ProductDetailResponse>;
