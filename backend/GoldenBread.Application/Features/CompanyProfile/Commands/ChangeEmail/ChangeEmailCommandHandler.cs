using GoldenBread.Application.Exceptions;
using GoldenBread.Application.Services;

namespace GoldenBread.Application.Features.CompanyProfile.Commands.ChangeEmail;

public sealed class ChangeEmailCommandHandler(
    ICurrentAccountContext accountContext,
    IUniquenessChecker checker,
    IPasswordHasher hasher) :
    IRequestHandler<ChangeEmailCommand, Unit>
{
    public async Task<Unit> Handle(
        ChangeEmailCommand command,
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
