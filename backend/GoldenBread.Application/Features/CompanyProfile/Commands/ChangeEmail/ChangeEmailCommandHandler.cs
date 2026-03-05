using GoldenBread.Application.Abstractions.Data;
using GoldenBread.Application.Abstractions.Services;
using GoldenBread.Application.Common.Exceptions.Auth;

namespace GoldenBread.Application.Features.CompanyProfile.Commands.ChangeEmail;

public sealed class ChangeEmailCommandHandler(
    IGoldenBreadContext context,
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

        await context.SaveChangesAsync(cancellationToken);

        return Unit.Value;
    }
}
