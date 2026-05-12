using GoldenBread.Application.Abstractions.Data;
using GoldenBread.Application.Abstractions.Data.Repositories;

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
