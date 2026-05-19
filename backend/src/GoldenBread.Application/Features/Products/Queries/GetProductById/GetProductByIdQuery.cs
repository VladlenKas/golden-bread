using GoldenBread.Application.Features.Products.Dtos;

namespace GoldenBread.Application.Features.Products.Queries.GetProductById;

public sealed record GetProductByIdQuery(int Id) : IRequest<ProductDto?>;
