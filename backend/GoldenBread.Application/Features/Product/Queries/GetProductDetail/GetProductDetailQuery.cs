using GoldenBread.Application.Features.Product.Dtos;

namespace GoldenBread.Application.Features.Product.Queries.GetProductDetail;

public sealed record class GetProductDetailQuery(int ProductId) :
    IRequest<ProductDetailResponse>;
