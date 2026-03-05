using GoldenBread.Domain.Entities;

namespace GoldenBread.Application.Abstractions.Services;

public interface ICurrentAccountContext
{
    Task<Account> GetAccountAsync(CancellationToken cancellationToken);
    Task<int> GetCompanyIdAsync(CancellationToken cancellationToken);
    string? GetSessionFromCookie();
    string? GetSessionToken();
}
