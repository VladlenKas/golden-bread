using GoldenBread.Domain.Entities;

namespace GoldenBread.Application.Abstractions.Data.Repositories;

public interface IProductRepository
{
    Task<List<Product>> GetAll(CancellationToken ct);
}
