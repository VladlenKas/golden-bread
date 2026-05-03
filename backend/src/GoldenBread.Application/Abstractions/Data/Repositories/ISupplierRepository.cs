using GoldenBread.Domain.Entities;

namespace GoldenBread.Application.Abstractions.Data.Repositories;

public interface ISupplierRepository
{
    Task<IReadOnlyList<Supplier>> GetAllAsync(CancellationToken ct = default);
    Task<Supplier?> GetByIdAsync(int id, CancellationToken ct = default);
    Task AddAsync(Supplier supplier, CancellationToken ct = default);
}