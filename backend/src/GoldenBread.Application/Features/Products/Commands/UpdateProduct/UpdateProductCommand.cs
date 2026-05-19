using GoldenBread.Application.Features.Products.Dtos;

namespace GoldenBread.Application.Features.Products.Commands.UpdateProduct;

public sealed record UpdateProductCommand(ProductDto Dto) : IRequest<bool>;
