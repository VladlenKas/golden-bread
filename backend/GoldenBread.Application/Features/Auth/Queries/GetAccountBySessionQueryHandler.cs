using GoldenBread.Application.Common.Abstractions.Data;
using GoldenBread.Application.Common.Abstractions.Services;

namespace GoldenBread.Application.Features.Auth.Queries;

public sealed class GetAccountBySessionQueryHandler(
    IGoldenBreadContext context,
    ICookieService cookieService)
    : IRequestHandler<GetAccountBySessionQuery, AuthResponse>
{
    public async Task<AuthResponse> Handle(
        GetAccountBySessionQuery query, 
        CancellationToken cancellationToken)
    {
        string? session = await cookieService.FindMeAsync();

        var account = await context.Accounts
            .FirstOrDefaultAsync(c =>
                c.Session == session &&
                c.SessionExpiresAt > DateTime.UtcNow,
                cancellationToken);

        if (account == null)
        {
            throw new UnauthorizedAccessException();
        }

        return new AuthResponse(
            account.AccountId,
            account.AccountType,
            account.VerificationStatus);
    }
}
