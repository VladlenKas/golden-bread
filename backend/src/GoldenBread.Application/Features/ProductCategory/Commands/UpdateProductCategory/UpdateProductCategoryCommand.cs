using GoldenBread.Application.Features.ProductCategory.Dtos;

namespace GoldenBread.Application.Features.ProductCategory.Commands.UpdateProductCategory;

public sealed record UpdateProductCategoryCommand(ProductCategoryDto Dto) : IRequest<bool>;
