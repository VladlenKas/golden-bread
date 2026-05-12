using GoldenBread.Application.Abstractions.Data.Repositories;
using GoldenBread.Application.Features.SupplierIngredient.Dtos;

namespace GoldenBread.Application.Features.SupplierIngredient.Queries.GetSupplierIngredientsList;

public sealed class GetSupplierIngredientsListQueryHandler(ISupplierIngredientRepository repository)
    : IRequestHandler<GetSupplierIngredientsListQuery, SupplierIngredientsListResponse>
{
    public async Task<SupplierIngredientsListResponse> Handle(GetSupplierIngredientsListQuery query, CancellationToken ct)
    {
        var entities = await repository.GetAllAsync(ct);

        var list = entities.Select(si => new SupplierIngredientListItem(
            si.SupplierIngredientId,
            si.Name,
            si.Supplier.Name,
            si.Ingredient.Name,
            si.Unit,
            si.Weight,
            si.Price,
            si.ShelfLifeDays,
            si.QuantityBatches,
            si.QuantityUnitInBatches))
            .ToList();

        return new SupplierIngredientsListResponse(list);
    }
}