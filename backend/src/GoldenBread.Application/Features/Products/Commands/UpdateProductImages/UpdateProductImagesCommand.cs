namespace GoldenBread.Application.Features.Products.Commands.UpdateProductImages;

public record UpdateProductImagesCommand(int ProductId, List<string> ImagePaths) : IRequest<bool>;
