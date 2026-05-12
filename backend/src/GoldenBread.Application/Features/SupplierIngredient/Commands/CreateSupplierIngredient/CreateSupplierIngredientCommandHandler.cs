using GoldenBread.Application.Abstractions.Data;
using GoldenBread.Application.Abstractions.Data.Repositories;

namespace GoldenBread.Application.Features.SupplierIngredient.Commands.CreateSupplierIngredient;

public sealed class CreateSupplierIngredientCommandHandler(
    ISupplierIngredientRepository repository,
    IUnitOfWork unitOfWork)
    : IRequestHandler<CreateSupplierIngredientCommand, int>
{
    public async Task<int> Handle(CreateSupplierIngredientCommand request, CancellationToken ct)
    {
        var dto = request.SupplierIngredientDto;
        var entity = DbEntities.SupplierIngredient.Create(
            0,
            dto.SupplierId,
            dto.IngredientId,
            dto.Name,
            dto.Price,
            dto.Unit,
            dto.Weight,
            dto.ShelfLifeDays);

        await repository.AddAsync(entity, ct);
        await unitOfWork.SaveChangesAsync(ct);

        return entity.SupplierIngredientId;
    }
}

