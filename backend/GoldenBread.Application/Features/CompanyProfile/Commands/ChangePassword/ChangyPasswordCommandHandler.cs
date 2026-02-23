using GoldenBread.Application.Exceptions;
using GoldenBread.Application.Services;

namespace GoldenBread.Application.Features.CompanyProfile.Commands.ChangePassword;

public sealed class ChangePasswordCommandHandler(
    ICurrentAccountContext accountContext,
    IPasswordHasher hasher) :
    IRequestHandler<ChangePasswordCommand, Unit>
{
    public async Task<Unit> Handle(
        ChangePasswordCommand command,
        CancellationToken cancellationToken)
    {
        var account = await accountContext.GetAccountAsync(cancellationToken);

        if (!hasher.Verify(command.OldPassword, account.PasswordHash))
            throw new PasswordsMismatchException();

        if (command.OldPassword == command.NewPassword)
            throw new NewPasswrodDuplicateException();

        account.UpdatePassword(hasher.Create(command.NewPassword));

        return Unit.Value;
    }
}
