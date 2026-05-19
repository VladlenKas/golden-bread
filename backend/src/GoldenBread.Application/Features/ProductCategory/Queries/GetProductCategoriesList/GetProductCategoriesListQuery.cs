using GoldenBread.Application.Features.ProductCategory.Dtos;

namespace GoldenBread.Application.Features.ProductCategory.Queries.GetProductCategoriesList;

public sealed record GetProductCategoriesListQuery : IRequest<ProductCategoriesListResponse>;
