using GoldenBread.Domain.Entities;

namespace GoldenBread.Application.Abstractions.Data.Repositories;

public interface ICompanyRepository
{
    Task<bool> ExistsByNameAsync(string name, int? excludeId = null, CancellationToken ct = default);
    Task<bool> ExistsByPhoneAsync(string? phone, int? excludeId = null, CancellationToken ct = default);
    Task<bool> ExistsByAddressAsync(string? address, int? excludeId = null, CancellationToken ct = default);
    Task<bool> ExistsByInnAsync(string inn, int? excludeId = null, CancellationToken ct = default);
    Task<bool> ExistsByOgrnAsync(string ogrn, int? excludeId = null, CancellationToken ct = default);
    Task<Company> AddAsync(Company company, CancellationToken ct);
    Task<IReadOnlyList<Account>> GetAllAsync(CancellationToken ct = default);
    Task<Company?> GetByIdAsync(int id, CancellationToken ct = default);
}
