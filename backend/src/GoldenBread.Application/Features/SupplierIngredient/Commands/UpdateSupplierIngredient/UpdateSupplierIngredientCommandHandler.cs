using GoldenBread.Application.Abstractions.Data;
using GoldenBread.Application.Abstractions.Data.Repositories;
using GoldenBread.Application.Common.Exceptions;

namespace GoldenBread.Application.Features.SupplierIngredient.Commands.UpdateSupplierIngredient;

public sealed class UpdateSupplierIngredientCommandHandler(
    ISupplierIngredientRepository repository,
    IUnitOfWork unitOfWork)
    : IRequestHandler<UpdateSupplierIngredientCommand, bool>
{
    public async Task<bool> Handle(UpdateSupplierIngredientCommand request, CancellationToken ct)
    {
        var dto = request.Dto;
        var entity = await repository.GetByIdAsync(dto.SupplierIngredientId, ct);

        if (await repository.ExistsByNameAsync(dto.SupplierId, dto.Name, dto.SupplierIngredientId, ct))
            throw new DuplicateEntityException(nameof(dto.Name));

        if (entity is null)
            return false;

        entity.Update(
            dto.SupplierId,
            dto.IngredientId,
            dto.Name,
            dto.Price,
            dto.Unit,
            dto.Weight,
            dto.ShelfLifeDays);

        await unitOfWork.SaveChangesAsync(ct);
        return true;
    }
}
