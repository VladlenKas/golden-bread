using GoldenBread.Application.Features.ProductCategory.Dtos;

namespace GoldenBread.Application.Features.ProductCategory.Queries.GetProductCategoryById;

public sealed record GetProductCategoryByIdQuery(int Id) : IRequest<ProductCategoryDto?>;
