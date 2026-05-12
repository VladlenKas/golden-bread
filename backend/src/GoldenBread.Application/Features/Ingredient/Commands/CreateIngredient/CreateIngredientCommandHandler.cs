using GoldenBread.Application.Abstractions.Data;
using GoldenBread.Application.Abstractions.Data.Repositories;


namespace GoldenBread.Application.Features.Ingredient.Commands.CreateIngredient;

public sealed class CreateIngredientCommandHandler(
    IIngredientRepository repository,
    IUnitOfWork unitOfWork)
    : IRequestHandler<CreateIngredientCommand, int>
{
    public async Task<int> Handle(CreateIngredientCommand request, CancellationToken ct)
    {
        var dto = request.IngredientDto;
        var entity = new DbEntities.Ingredient
        {
            Name = dto.Name,
            BaseUnit = dto.BaseUnit
        };

        await repository.AddAsync(entity, ct);
        await unitOfWork.SaveChangesAsync(ct);

        return entity.IngredientId;
    }
}