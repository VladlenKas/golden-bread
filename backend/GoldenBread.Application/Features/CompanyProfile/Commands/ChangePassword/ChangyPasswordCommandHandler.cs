using GoldenBread.Application.Abstractions.Data;
using GoldenBread.Application.Abstractions.Services;
using GoldenBread.Application.Common.Exceptions.Auth;

namespace GoldenBread.Application.Features.CompanyProfile.Commands.ChangePassword;

public sealed class ChangePasswordCommandHandler(
    IGoldenBreadContext context,
    ICurrentAccountContext accountContext,
    IPasswordHasher hasher) :
    IRequestHandler<ChangePasswordCommand, Unit>
{
    public async Task<Unit> Handle(
        ChangePasswordCommand command,
        CancellationToken ct)
    {
        var account = await accountContext.GetAccountAsync(ct);

        if (!hasher.Verify(command.OldPassword, account.PasswordHash))
            throw new PasswordsMismatchException();

        if (command.OldPassword == command.NewPassword)
            throw new PasswordSameAsCurrentException();

        account.UpdatePassword(hasher.Create(command.NewPassword));
        
        await context.SaveChangesAsync(ct);

        return Unit.Value;
    }
}
