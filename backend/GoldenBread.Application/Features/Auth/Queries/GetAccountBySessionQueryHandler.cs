using GoldenBread.Application.Common.Abstractions.Data;
using GoldenBread.Application.Common.Abstractions.Services;

namespace GoldenBread.Application.Features.Auth.Queries;

public sealed class GetAccountBySessionQueryHandler(
    IGoldenBreadContext context,
    ISessionService sessionService)
    : IRequestHandler<GetAccountBySessionQuery, AuthResponse?>
{
    public async Task<AuthResponse?> Handle(
        GetAccountBySessionQuery request, 
        CancellationToken cancellationToken)
    {
        return null;
    }
}
