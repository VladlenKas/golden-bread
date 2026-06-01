using GoldenBread.Domain.Enums;

namespace GoldenBread.Application.Features.Ingredient.Dtos;

public record IngredientListItem(
    int IngredientId,
    string Name,
    IngredientUnit BaseUnit,
    int RecipesCount);

public record IngredientsListResponse(List<IngredientListItem> IngredientsList);
