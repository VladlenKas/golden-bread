using GoldenBread.Application.Abstractions.Services;
using GoldenBread.Application.Features.Auth.Dtos;

namespace GoldenBread.Application.Features.Auth.Queries.GetAccountBySession;

public sealed class GetAccountBySessionQueryHandler(ICurrentAccountContext accountContext) : 
    IRequestHandler<GetAccountBySessionQuery, AuthResponse?>
{
    public async Task<AuthResponse?> Handle(
        GetAccountBySessionQuery query, 
        CancellationToken ct)
    {
        var session = accountContext.GetSessionFromCookie(); 

        if (string.IsNullOrEmpty(session))
            return null;

        var account = await accountContext.GetAccountAsync(ct);

        return new AuthResponse(
            account.AccountId,
            account.VerificationStatus);
    }
}
