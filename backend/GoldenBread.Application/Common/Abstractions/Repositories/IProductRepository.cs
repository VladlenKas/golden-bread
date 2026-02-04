using GoldenBread.Domain.Entities;

namespace GoldenBread.Application.Common.Abstractions.Repositories;

public interface IProductRepository
{
    Task<IEnumerable<User?>> GetAllAsync();
    Task<Product?> GetByIdAsync(int id);
    Task AddAsync(Product product);
    Task UpdateAsync(Product product);
    Task SoftDeleteAsync(Product product);
}
