using GoldenBread.Domain.Entities;

namespace GoldenBread.Application.Abstractions.Services;

public interface ICurrentAccountContext
{
    Task<Account> GetAccountAsync(CancellationToken ct);
    Task<int> GetCompanyIdAsync(CancellationToken ct);
    string? GetSessionFromCookie();
    string? GetSessionToken();
}
