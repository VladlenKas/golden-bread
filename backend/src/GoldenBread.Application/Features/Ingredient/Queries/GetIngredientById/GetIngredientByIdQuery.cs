using GoldenBread.Application.Features.Ingredient.Dtos;

namespace GoldenBread.Application.Features.Ingredient.Queries.GetIngredientById;

public sealed record GetIngredientByIdQuery(int Id) : IRequest<IngredientDto?>;