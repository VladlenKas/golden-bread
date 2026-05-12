using GoldenBread.Application.Abstractions.Data;
using GoldenBread.Application.Abstractions.Data.Repositories;


namespace GoldenBread.Application.Features.Ingredient.Commands.UpdateIngredient;

public sealed class UpdateIngredientCommandHandler(
    IIngredientRepository repository,
    IUnitOfWork unitOfWork)
    : IRequestHandler<UpdateIngredientCommand, bool>
{
    public async Task<bool> Handle(UpdateIngredientCommand request, CancellationToken ct)
    {
        var dto = request.IngredientDto;
        var entity = await repository.GetByIdAsync(dto.IngredientId, ct);

        if (entity is null)
            return false;

        entity.Name = dto.Name;
        entity.BaseUnit = dto.BaseUnit;

        await unitOfWork.SaveChangesAsync(ct);
        return true;
    }
}