namespace GoldenBread.Application.Features.Ingredient.Commands.DeleteIngredient;

public sealed record DeleteIngredientCommand(int IngredientId) : IRequest<bool>;