using GoldenBread.Application.Features.Ingredient.Dtos;

namespace GoldenBread.Application.Features.Ingredient.Commands.UpdateIngredient;

public sealed record UpdateIngredientCommand(IngredientDto IngredientDto) : IRequest<bool>;
