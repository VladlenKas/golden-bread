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
        CancellationToken ct)
    {
        Account account = await accountContext
            .GetAccountAsync(ct);

        account.ClearSession();
        await cookieService.SignOutAsync();
        await context.SaveChangesAsync(ct);

        return Unit.Value;
    }
}
