using GoldenBread.Application.Abstractions.Data;
using GoldenBread.Application.Abstractions.Services;
using GoldenBread.Application.Common.Exceptions.Auth;

namespace GoldenBread.Application.Features.CompanyProfile.Commands.ChangePassword;

public sealed class ChangePasswordCommandHandler(
    IUnitOfWork unitOfWork,
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

        if (command.NewPassword == command.OldPassword)
            throw new NewPasswordSameAsOldException();

        account.UpdatePassword(hasher.Create(command.NewPassword));
        
        await unitOfWork.SaveChangesAsync(ct);

        return Unit.Value;
    }
}
