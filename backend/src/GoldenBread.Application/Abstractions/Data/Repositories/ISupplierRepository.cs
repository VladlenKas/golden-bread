using GoldenBread.Domain.Entities;

namespace GoldenBread.Application.Abstractions.Data.Repositories;

public interface ISupplierRepository
{
    Task<bool> ExistsByNameAsync(string name, int? excludeId = null, CancellationToken ct = default);
    Task<bool> ExistsByEmailAsync(string? email, int? excludeId = null, CancellationToken ct = default);
    Task<bool> ExistsByPhoneAsync(string? phone, int? excludeId = null, CancellationToken ct = default);
    Task<bool> ExistsByAddressAsync(string? adsress, int? excludeId = null, CancellationToken ct = default);
    Task<IReadOnlyList<Supplier>> GetAllAsync(CancellationToken ct = default);
    Task<Supplier?> GetByIdAsync(int id, CancellationToken ct = default);
    Task AddAsync(Supplier supplier, CancellationToken ct = default);
}