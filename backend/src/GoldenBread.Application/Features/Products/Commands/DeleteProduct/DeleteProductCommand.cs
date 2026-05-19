namespace GoldenBread.Application.Features.Products.Commands.DeleteProduct;

public sealed record DeleteProductCommand(int ProductId) : IRequest<bool>;
