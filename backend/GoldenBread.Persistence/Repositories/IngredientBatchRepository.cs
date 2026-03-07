using GoldenBread.Application.Abstractions.Data;
using GoldenBread.Application.Abstractions.Repositories;
using GoldenBread.Domain.Entities;
using GoldenBread.Domain.Enums;

namespace GoldenBread.Infrastructure.Repositories;

public class IngredientBatchRepository(IGoldenBreadContext context) : IIngredientBatchRepository
{
    public async Task<IReadOnlyList<IngredientBatch>> GetAvailableForIngredientAsync(
        int ingredientId,
        CancellationToken cancellationToken = default)
    {
        return await context.IngredientBatches
            .Where(ib => ib.IngredientId == ingredientId)
            .Where(ib => ib.Status == IngredientBatchStatus.Available)
            .Where(ib => ib.RemainingQuantity > 0)
            .OrderBy(ib => ib.DeliveryDate)
            .AsNoTracking()
            .ToListAsync(cancellationToken);
    }
}

