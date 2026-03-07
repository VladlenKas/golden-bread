using GoldenBread.Domain.Entities;

namespace GoldenBread.Application.Abstractions.Repositories;

public interface IIngredientBatchRepository
{
    Task<IReadOnlyList<IngredientBatch>> GetAvailableForIngredientAsync(int ingredientId, CancellationToken cancellationToken = default);
}

