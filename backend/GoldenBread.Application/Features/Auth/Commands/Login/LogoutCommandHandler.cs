using GoldenBread.Application.Services;

namespace GoldenBread.Application.Features.Auth.Commands.Login;

public sealed class LogoutCommandHandler(ICookieService cookieService) 
    : IRequestHandler<LogoutCommand, Unit>
{
    public async Task<Unit> Handle(
        LogoutCommand command, 
        CancellationToken cancellationToken)
    {
        command.Account.ClearSession();
        await cookieService.SignOutAsync();
        return Unit.Value;
    }
}
