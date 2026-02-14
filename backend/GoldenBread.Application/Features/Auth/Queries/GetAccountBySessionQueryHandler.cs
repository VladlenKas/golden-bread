using GoldenBread.Application.Common.Abstractions.Services;

namespace GoldenBread.Application.Features.Auth.Queries;

public sealed class GetAccountBySessionQueryHandler(
    ICurrentUserService currentUserService)
    : IRequestHandler<GetAccountBySessionQuery, AuthResponse>
{
    public async Task<AuthResponse> Handle(
        GetAccountBySessionQuery query, 
        CancellationToken cancellationToken)
    {
        var account = await currentUserService.Account(cancellationToken);

        if (account == null)
            throw new UnauthorizedAccessException();

        return new AuthResponse(
            account.AccountId,
            account.AccountType,
            account.VerificationStatus);
    }
}
