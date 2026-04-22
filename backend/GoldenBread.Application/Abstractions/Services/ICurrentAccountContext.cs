using GoldenBread.Domain.Entities;

namespace GoldenBread.Application.Abstractions.Services;

public interface ICurrentAccountContext
{
    bool HasSession { get; }
    Task<Account> GetAccountAsync(CancellationToken ct);
    Task<int?> GetCompanyIdAsync(CancellationToken ct);
    Task<int> GetRequiredCompanyIdAsync(CancellationToken ct);
}
