using GoldenBread.Application.Abstractions.Data;
using GoldenBread.Application.Abstractions.Services;
using GoldenBread.Application.Common.Exceptions.Auth;
using GoldenBread.Domain.Entities;
using Microsoft.AspNetCore.Http;

namespace GoldenBread.Infrastructure.Services;

internal class CurrentAccountContext(
    IHttpContextAccessor httpContextAccessor,
    IGoldenBreadContext context) : 
    ICurrentAccountContext
{
    private Task<Account?>? _accountCache;

    public string? GetSessionToken() => httpContextAccessor
        .HttpContext?
        .User
        .FindFirst("session")?.Value;

    public string? GetSessionFromCookie() => httpContextAccessor
        .HttpContext?
        .Request
        .Cookies["gb.session"]; 

    public async Task<Account> GetAccountAsync(CancellationToken cancellationToken)
    {
        _accountCache ??= LoadAccountAsync(cancellationToken);
        return await _accountCache ?? throw new SessionExpiredException();
    }

    private async Task<Account?> LoadAccountAsync(CancellationToken cancellationToken)
    {
        var session = GetSessionToken();
        if (session == null)
            return null;

        return await context.Accounts
            .FirstOrDefaultAsync(a => 
                a.Session == session &&
                a.SessionExpiresAt > DateTime.UtcNow,
                cancellationToken);
    }

    public async Task<int> GetCompanyIdAsync(CancellationToken cancellationToken)
    {
        var session = GetSessionToken();
        if (string.IsNullOrEmpty(session)) return 0;

        var account = await GetAccountAsync(cancellationToken);
        return account.Company.CompanyId;
    }
}
