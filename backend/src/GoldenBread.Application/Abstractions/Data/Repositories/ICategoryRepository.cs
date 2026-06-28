using GoldenBread.Domain.Entities;

namespace GoldenBread.Application.Abstractions.Data.Repositories;

public interface ICategoryRepository
{
    Task<List<ProductCategory>> GetAllAsync(CancellationToken ct);
    Task<ProductCategory?> GetByIdAsync(int id, CancellationToken ct = default);
    Task AddAsync(ProductCategory category, CancellationToken ct = default);
    Task<bool> ExistsByNameAsync(string name, int? excludeId = null, CancellationToken ct = default);
}
