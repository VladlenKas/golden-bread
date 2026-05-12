using GoldenBread.Application.Abstractions.Data;
using GoldenBread.Application.Features.Ingredient.Dtos;

namespace GoldenBread.Application.Features.Ingredient.Queries.GetIngredientsList;

public sealed class GetIngredientsListQueryHandler(IGoldenBreadContext context)
    : IRequestHandler<GetIngredientsListQuery, IngredientsListResponse>
{
    public async Task<IngredientsListResponse> Handle(GetIngredientsListQuery query, CancellationToken ct)
    {
        var list = await context.Ingredients
            .Where(i => i.DeletedAt == null)
            .Select(i => new IngredientListItem(
                i.IngredientId,
                i.Name,
                i.BaseUnit,
                i.Recipes.Count,
                i.SupplierIngredients.Count(si => si.DeletedAt == null)))
            .ToListAsync(ct);

        return new IngredientsListResponse(list);
    }
}