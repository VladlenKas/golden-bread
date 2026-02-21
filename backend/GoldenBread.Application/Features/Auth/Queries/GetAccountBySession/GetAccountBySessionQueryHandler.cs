using GoldenBread.Application.Features.Auth.Dtos;
using GoldenBread.Application.Services;

namespace GoldenBread.Application.Features.Auth.Queries.GetAccountBySession;

public sealed class GetAccountBySessionQueryHandler(ICurrentAccountContext accountContext) : 
    IRequestHandler<GetAccountBySessionQuery, AuthResponse?>
{
    public async Task<AuthResponse?> Handle(
        GetAccountBySessionQuery query, 
        CancellationToken cancellationToken)
    {
        var session = accountContext.GetSessionFromCookie(); 

        if (string.IsNullOrEmpty(session))
            return null;

        var account = await accountContext.GetAccountAsync(cancellationToken);

        return new AuthResponse(
            account.AccountId,
            account.VerificationStatus);
    }
}
