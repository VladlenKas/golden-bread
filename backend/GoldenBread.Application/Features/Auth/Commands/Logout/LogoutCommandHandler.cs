using GoldenBread.Application.Abstractions.Data;
using GoldenBread.Application.Abstractions.Services;
using GoldenBread.Domain.Entities;

namespace GoldenBread.Application.Features.Auth.Commands.Logout;

public sealed class LogoutCommandHandler(
    IGoldenBreadContext context,
    ICookieService cookieService,
    ICurrentAccountContext accountContext) : 
    IRequestHandler<LogoutCommand, Unit>
{
    public async Task<Unit> Handle(
        LogoutCommand command, 
        CancellationToken cancellationToken)
    {
        Account account = await accountContext
            .GetAccountAsync(cancellationToken);

        account.ClearSession();
        await cookieService.SignOutAsync();
        await context.SaveChangesAsync(cancellationToken);

        return Unit.Value;
    }
}
