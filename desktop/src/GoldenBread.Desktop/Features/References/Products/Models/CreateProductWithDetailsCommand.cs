namespace GoldenBread.Desktop.Features.References.Products.Models;

public record CreateProductWithDetailsCommand(
    ProductDto Product,
    List<RecipeItemDto> Recipes,
    List<ProductBatchDto> Batches,
    List<string> ImagePaths);