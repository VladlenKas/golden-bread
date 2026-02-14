using GoldenBread.Application.Common.Abstractions.Data;
using GoldenBread.Application.Common.Abstractions.Services;

namespace GoldenBread.Application.Features.Auth.Commands.Login;

public sealed class LogoutCommandHandler(
    ICurrentUserService currentUserService,
    ICookieService cookieService) 
    : IRequestHandler<LogoutCommand, Unit>
{
    public async Task<Unit> Handle(
        LogoutCommand command, 
        CancellationToken cancellationToken)
    {
        var account = await currentUserService.Account(cancellationToken);

        if (account == null)
            throw new UnauthorizedAccessException();

        account.ClearSession();

        await cookieService.SignOutAsync();

        return Unit.Value;
    }
}
