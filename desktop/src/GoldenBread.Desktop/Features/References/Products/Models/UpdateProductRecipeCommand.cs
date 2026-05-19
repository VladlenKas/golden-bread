namespace GoldenBread.Desktop.Features.References.Products.Models;

public record UpdateProductRecipeCommand(int ProductId, List<RecipeItemDto> Recipes);
