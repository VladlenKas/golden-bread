using GoldenBread.Application.Abstractions.Data;
using GoldenBread.Application.Abstractions.Data.Repositories;
using GoldenBread.Domain.Entities;

namespace GoldenBread.Infrastructure.Data.Repositories;

public class ProductBatchRepository(IGoldenBreadContext context) : IProductBatchRepository
{
    public async Task<IReadOnlyList<ProductBatch>> GetByProductIdAsync(int productId, CancellationToken ct = default)
    {
        return await context.ProductBatches
            .Where(pb => pb.ProductId == productId)
            .ToListAsync(ct);
    }

    public async Task AddAsync(ProductBatch batch, CancellationToken ct = default)
    {
        await context.ProductBatches.AddAsync(batch, ct);
    }
}