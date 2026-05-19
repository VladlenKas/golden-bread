namespace GoldenBread.Desktop.Features.References.Products.Models;

public record UpdateProductImagesCommand(int ProductId, List<string> ImagePaths);
