namespace GoldenBread.Application.Features.ProductCategory.Commands.DeleteProductCategory;

public sealed record DeleteProductCategoryCommand(int ProductCategoryId) : IRequest<bool>;