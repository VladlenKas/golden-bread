using GoldenBread.Application.Common.Abstractions.Data;
using GoldenBread.Application.Common.Abstractions.Services;
using GoldenBread.Domain.Entities;
using Microsoft.AspNetCore.Http;

namespace GoldenBread.Infrastructure.Services;

internal class CurrentUserService(
    IHttpContextAccessor httpContextAccessor,
    IGoldenBreadContext context) : ICurrentUserService
{
    private Task<Account?>? _accountCache;

    private string? SessionToken => httpContextAccessor.HttpContext?
        .User.FindFirst("session")?.Value;

    public async Task<Account?> Account(CancellationToken cancellationToken)
    {
        _accountCache ??= LoadAccountAsync(cancellationToken);
        var account = await _accountCache;
        return account ?? throw new KeyNotFoundException();
    }

    private async Task<Account?> LoadAccountAsync(CancellationToken cancellationToken)
    {
        var session = SessionToken;
        if (session == null)
            return null;

        return await context.Accounts
            .AsNoTracking()
            .FirstOrDefaultAsync(a => 
                a.Session == session &&
                a.SessionExpiresAt > DateTime.UtcNow,
                cancellationToken);
    }
}
