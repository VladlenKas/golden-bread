using GoldenBread.Application.Exceptions;
using GoldenBread.Application.Services;

namespace GoldenBread.Application.Features.CompanyAccount.Commands.ChangeMyPassword;

public sealed class ChangePasswordCommandHandler(
    ICurrentAccountContext accountContext,
    IPasswordHasher hasher) :
    IRequestHandler<ChangeMyPasswordCommand, Unit>
{
    public async Task<Unit> Handle(
        ChangeMyPasswordCommand command,
        CancellationToken cancellationToken)
    {
        var account = await accountContext.GetAccountAsync(cancellationToken);

        if (!hasher.Verify(command.OldPassword, account.PasswordHash))
            throw new PasswordsMismatchException();

        account.UpdatePassword(hasher.Create(command.NewPassword));

        return Unit.Value;
    }
}
