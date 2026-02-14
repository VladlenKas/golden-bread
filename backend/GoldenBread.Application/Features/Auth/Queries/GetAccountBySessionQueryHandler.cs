using GoldenBread.Application.Common.Abstractions.Services;

namespace GoldenBread.Application.Features.Auth.Queries;

public sealed class GetAccountBySessionQueryHandler()
    : IRequestHandler<GetAccountBySessionQuery, AuthResponse>
{
    public async Task<AuthResponse> Handle(
        GetAccountBySessionQuery query, 
        CancellationToken cancellationToken)
    {
        return new AuthResponse(
            query.Account.AccountId,
            query.Account.AccountType,
            query.Account.VerificationStatus);
    }
}
