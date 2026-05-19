using GoldenBread.Domain.Entities;

namespace GoldenBread.Application.Abstractions.Data.Repositories;

public interface IProductRepository
{
    Task<List<Product>> GetAll(CancellationToken ct);
    Task<Product?> GetByIdAsync(int id, CancellationToken ct = default);
    Task<Product?> GetByIdWithDetailsAsync(int id, CancellationToken ct = default);
    Task AddAsync(Product product, CancellationToken ct = default);
}
