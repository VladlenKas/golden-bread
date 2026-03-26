using GoldenBread.Domain.Entities;

namespace GoldenBread.Application.Abstractions.Data.Repositories;

public interface ICategoryRepository
{
    Task<List<ProductCategory>> GetAll(CancellationToken ct);
}
