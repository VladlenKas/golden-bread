using GoldenBread.Application.Exceptions;
using GoldenBread.Application.Services;

namespace GoldenBread.Application.Features.CompanyAccount.Commands.ChangeMyEmail;

public sealed class ChangeMyEmailCommandHandler(
    ICurrentAccountContext accountContext,
    IUniquenessChecker checker,
    IPasswordHasher hasher) :
    IRequestHandler<ChangeMyEmailCommand, Unit>
{
    public async Task<Unit> Handle(
        ChangeMyEmailCommand command,
        CancellationToken cancellationToken)
    {
        var account = await accountContext.GetAccountAsync(cancellationToken);

        if (!hasher.Verify(command.Password, account.PasswordHash)) 
            throw new PasswordsMismatchException();
        await checker.EmailMustBeUniqueAsync(command.NewEmail, account.AccountId, cancellationToken);

        account.UpdateEmail(command.NewEmail);
        account.ClearSession();

        return Unit.Value;
    }
}
