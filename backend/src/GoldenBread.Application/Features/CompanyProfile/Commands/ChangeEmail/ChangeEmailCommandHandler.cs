using GoldenBread.Application.Abstractions.Data;
using GoldenBread.Application.Abstractions.Data.Repositories;
using GoldenBread.Application.Abstractions.Services;
using GoldenBread.Application.Common.Constants;
using GoldenBread.Application.Common.Exceptions;

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
            throw new BusinessValidationException(nameof(command.Password), ValidationErrorConstants.InvalidPassword);

        if (await accountRepository.ExistsByEmailAsync(command.NewEmail, account.AccountId, ct))
            throw new DuplicateEntityException(nameof(command.NewEmail));

        account.UpdateEmail(command.NewEmail);
        account.ClearSession();

        await unitOfWork.SaveChangesAsync(ct);

        return Unit.Value;
    }
}
