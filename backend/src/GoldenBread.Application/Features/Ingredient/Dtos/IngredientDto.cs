using GoldenBread.Domain.Enums;

namespace GoldenBread.Application.Features.Ingredient.Dtos;

public record class IngredientDto(
    int IngredientId,
    string Name,
    IngredientUnit BaseUnit);
