using GoldenBread.Application.Abstractions.Data;
using GoldenBread.Application.Abstractions.Services;
using GoldenBread.Domain.Entities;
using GoldenBread.Domain.Enums;

namespace GoldenBread.Application.Features.Auth.Commands.Logout;

public sealed class LogoutCommandHandler(
    IUnitOfWork unitOfWork,
    ICookieService cookieService,
    ICurrentAccountContext accountContext) : 
    IRequestHandler<LogoutCommand, Unit>
{
    public async Task<Unit> Handle(
        LogoutCommand command, 
        CancellationToken ct)
    {
        Account account = await accountContext.GetAccountAsync(ct);
            
        account.ClearSession();
        await unitOfWork.SaveChangesAsync(ct);

        // Очищаем сессию в HttpContextAccessor только web-пользователей
        if (account.AccountType == AccountType.Company)
            await cookieService.SignOutAsync();

        return Unit.Value;
    }
}
