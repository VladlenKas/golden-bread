using GoldenBread.Application.Abstractions.Data;
using GoldenBread.Application.Abstractions.Data.Repositories;
using GoldenBread.Application.Common.Exceptions;

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

        if (await repository.ExistsByNameAsync(dto.SupplierId, dto.Name, null, ct))
            throw new DuplicateEntityException(nameof(dto.Name));

        await repository.AddAsync(entity, ct);
        await unitOfWork.SaveChangesAsync(ct);

        return entity.SupplierIngredientId;
    }
}

