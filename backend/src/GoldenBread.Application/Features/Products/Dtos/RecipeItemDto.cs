namespace GoldenBread.Application.Features.Products.Dtos;

public record RecipeItemDto(
    int? RecipeId,
    int IngredientId,
    decimal Quantity);
