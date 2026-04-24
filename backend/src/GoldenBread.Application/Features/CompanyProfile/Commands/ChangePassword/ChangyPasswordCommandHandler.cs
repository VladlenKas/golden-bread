using GoldenBread.Application.Abstractions.Data;
using GoldenBread.Application.Abstractions.Services;
using GoldenBread.Application.Common.Constants;
using GoldenBread.Application.Common.Exceptions;

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
            throw new BusinessValidationException(nameof(command.OldPassword), ValidationErrorConstants.PasswordsMismatch);

        if (command.NewPassword == command.OldPassword)
            throw new BusinessValidationException(nameof(command.NewPassword), ValidationErrorConstants.NewPasswordSameAsOld);

        account.UpdatePassword(hasher.Create(command.NewPassword));
        
        await unitOfWork.SaveChangesAsync(ct);

        return Unit.Value;
    }
}
