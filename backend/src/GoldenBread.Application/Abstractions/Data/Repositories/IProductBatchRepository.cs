using GoldenBread.Domain.Entities;

namespace GoldenBread.Application.Abstractions.Data.Repositories;

public interface IProductBatchRepository
{
    Task<IReadOnlyList<ProductBatch>> GetByProductIdAsync(int productId, CancellationToken ct = default);
    Task AddAsync(ProductBatch batch, CancellationToken ct = default);
}