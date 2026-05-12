using GoldenBread.Application.Features.Ingredient.Dtos;


namespace GoldenBread.Application.Features.Ingredient.Queries.GetIngredientsList;

public sealed record GetIngredientsListQuery : IRequest<IngredientsListResponse>;