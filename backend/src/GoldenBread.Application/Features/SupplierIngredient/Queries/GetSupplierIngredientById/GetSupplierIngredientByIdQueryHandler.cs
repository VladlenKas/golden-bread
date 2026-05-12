using GoldenBread.Application.Abstractions.Data.Repositories;
using GoldenBread.Application.Features.SupplierIngredient.Dtos;

namespace GoldenBread.Application.Features.SupplierIngredient.Queries.GetSupplierIngredientById;

public sealed class GetSupplierIngredientByIdQueryHandler(ISupplierIngredientRepository repository)
    : IRequestHandler<GetSupplierIngredientByIdQuery, SupplierIngredientDto?>
{
    public async Task<SupplierIngredientDto?> Handle(GetSupplierIngredientByIdQuery query, CancellationToken ct)
    {
        var entity = await repository.GetByIdAsync(query.Id, ct);
        if (entity is null)
            return null;

        return new SupplierIngredientDto(
            entity.SupplierIngredientId,
            entity.SupplierId,
            entity.IngredientId,
            entity.Name,
            entity.Price,
            entity.Unit,
            entity.Weight,
            entity.ShelfLifeDays);
    }
}