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
        CancellationToken ct)
    {
        var account = await accountContext.GetAccountAsync(ct);

        if (!hasher.Verify(command.Password, account.PasswordHash)) 
            throw new PasswordsMismatchException();
        await checker.EmailMustBeUniqueAsync(command.NewEmail, account.AccountId, ct);

        account.UpdateEmail(command.NewEmail);
        account.ClearSession();

        await context.SaveChangesAsync(ct);

        return Unit.Value;
    }
}
