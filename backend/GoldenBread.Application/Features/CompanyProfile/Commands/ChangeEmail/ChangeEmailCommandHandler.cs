using GoldenBread.Application.Abstractions.Data;
using GoldenBread.Application.Abstractions.Repositories;
using GoldenBread.Application.Abstractions.Services;
using GoldenBread.Application.Common.Exceptions.Auth;
using GoldenBread.Application.Common.Exceptions.Domain;

namespace GoldenBread.Application.Features.CompanyProfile.Commands.ChangeEmail;

public sealed class ChangeEmailCommandHandler(
    IUnitOfWork unitOfWork,
    IAccountRepository accountRepository,
    ICurrentAccountContext accountContext,
    IPasswordHasher hasher) :
    IRequestHandler<ChangeEmailCommand, Unit>
{
    public async Task<Unit> Handle(
        ChangeEmailCommand command,
        CancellationToken ct)
    {
        var account = await accountContext.GetAccountAsync(ct);

        if (!hasher.Verify(command.Password, account.PasswordHash)) 
            throw new PasswordsMismatchException();

        if (await accountRepository.ExistsByEmailAsync(command.NewEmail, account.AccountId, ct))
            throw new EmailDuplicateException();

        account.UpdateEmail(command.NewEmail);
        account.ClearSession();

        await unitOfWork.SaveChangesAsync(ct);

        return Unit.Value;
    }
}
