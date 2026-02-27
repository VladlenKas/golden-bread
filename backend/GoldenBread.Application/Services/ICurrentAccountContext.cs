using GoldenBread.Domain.Entities;

namespace GoldenBread.Application.Services;

public interface ICurrentAccountContext
{
    Task<Account> GetAccountAsync(CancellationToken cancellationToken);
    string? GetSessionFromCookie();
    string? GetSessionToken();
}
