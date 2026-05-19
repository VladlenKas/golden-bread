using GoldenBread.Application.Features.ProductCategory.Dtos;

namespace GoldenBread.Application.Features.ProductCategory.Commands.CreateProductCategory;

public sealed record CreateProductCategoryCommand(ProductCategoryDto Dto) : IRequest<int>;
