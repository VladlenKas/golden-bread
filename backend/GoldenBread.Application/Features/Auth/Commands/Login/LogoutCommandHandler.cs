using GoldenBread.Application.Common.Abstractions.Data;
using GoldenBread.Application.Common.Abstractions.Services;

namespace GoldenBread.Application.Features.Auth.Commands.Login;

public sealed class LogoutCommandHandler(
    IGoldenBreadContext context,
    ICookieService cookieService) 
    : IRequestHandler<LogoutCommand, Unit>
{
    public async Task<Unit> Handle(
        LogoutCommand command, 
        CancellationToken cancellationToken)
    {
        string? session = cookieService.GetSession();

        if (session == null)
            throw new KeyNotFoundException(nameof(session));

        var account = await context.Accounts
            .FirstOrDefaultAsync(c =>
                c.Session == session &&
                c.SessionExpiresAt > DateTime.UtcNow,
                cancellationToken);

        if (account == null)
            throw new UnauthorizedAccessException();

        account.ClearSession();

        await cookieService.SignOutAsync();

        return Unit.Value;
    }
}
