using GoldenBread.Application.Features.Products.Dtos;

namespace GoldenBread.Application.Features.Products.Commands.CreateProductWithDetails;

public record CreateProductWithDetailsCommand(
    ProductDto Product,
    List<RecipeItemDto> Recipes,
    List<ProductBatchDto> Batches,
    List<string> ImagePaths) : IRequest<int>;
