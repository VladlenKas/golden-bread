using GoldenBread.Application.Abstractions.Data;
using GoldenBread.Application.Abstractions.Data.Repositories;

namespace GoldenBread.Application.Features.SupplierIngredient.Commands.DeleteSupplierIngredient;

public sealed class DeleteSupplierIngredientCommandHandler(
    ISupplierIngredientRepository repository,
    IUnitOfWork unitOfWork)
    : IRequestHandler<DeleteSupplierIngredientCommand, bool>
{
    public async Task<bool> Handle(DeleteSupplierIngredientCommand request, CancellationToken ct)
    {
        var entity = await repository.GetByIdAsync(request.SupplierIngredientId, ct);

        if (entity is null)
            return false;

        entity.SoftDelete();

        foreach (var batch in entity.IngredientBatches)
        {
            batch.SetArchivedStatus(); // Архивируем складские позиции
        }

        await unitOfWork.SaveChangesAsync(ct);
        return true;
    }
}