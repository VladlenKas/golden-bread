using GoldenBread.Application.Abstractions.Data.Repositories;
using GoldenBread.Application.Features.Ingredient.Dtos;

namespace GoldenBread.Application.Features.Ingredient.Queries.GetIngredientById;

public sealed class GetIngredientByIdQueryHandler(IIngredientRepository repository)
    : IRequestHandler<GetIngredientByIdQuery, IngredientDto?>
{
    public async Task<IngredientDto?> Handle(GetIngredientByIdQuery query, CancellationToken ct)
    {
        var entity = await repository.GetByIdAsync(query.Id, ct);
        if (entity is null)
            return null;

        return new IngredientDto(
            entity.IngredientId,
            entity.Name,
            entity.BaseUnit);
    }
}