using GoldenBread.Application.Features.Auth.Dtos;
using GoldenBread.Application.Services;
using GoldenBread.Domain.Entities;

namespace GoldenBread.Application.Features.Auth.Queries.GetAccountBySession;

public sealed class GetAccountBySessionQueryHandler(ICurrentAccountContext accountContext) : 
    IRequestHandler<GetAccountBySessionQuery, AuthResponse>
{
    public async Task<AuthResponse> Handle(
        GetAccountBySessionQuery query, 
        CancellationToken cancellationToken)
    {
        Account account = await accountContext.GetAccountAsync(cancellationToken);

        return new AuthResponse(
            account.AccountId,
            account.VerificationStatus);
    }
}
