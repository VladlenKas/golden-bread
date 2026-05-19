using GoldenBread.Application.Features.Products.Dtos;

namespace GoldenBread.Application.Features.Products.Commands.UpdateProductRecipe;

public record UpdateProductRecipeCommand(int ProductId, List<RecipeItemDto> Recipes) : IRequest<bool>;
