using GoldenBread.Application.Features.Ingredient.Dtos;

namespace GoldenBread.Application.Features.Ingredient.Commands.CreateIngredient;

public sealed record CreateIngredientCommand(IngredientDto IngredientDto) : IRequest<int>;
