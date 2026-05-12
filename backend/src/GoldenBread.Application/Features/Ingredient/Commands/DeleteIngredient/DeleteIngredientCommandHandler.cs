using GoldenBread.Application.Abstractions.Data;
using GoldenBread.Application.Abstractions.Data.Repositories;

namespace GoldenBread.Application.Features.Ingredient.Commands.DeleteIngredient;

public sealed class DeleteIngredientCommandHandler(
    IIngredientRepository repository,
    IUnitOfWork unitOfWork)
    : IRequestHandler<DeleteIngredientCommand, bool>
{
    public async Task<bool> Handle(DeleteIngredientCommand request, CancellationToken ct)
    {
        var entity = await repository.GetByIdAsync(request.IngredientId, ct);

        if (entity is null)
            return false;

        // Soft-delete ингредиента
        entity.SoftDelete();

        await unitOfWork.SaveChangesAsync(ct);
        return true;
    }
}