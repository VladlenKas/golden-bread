using GoldenBread.Application.Abstractions.Data;
using GoldenBread.Application.Abstractions.Data.Repositories;
using GoldenBread.Application.Abstractions.Services;

namespace GoldenBread.Application.Features.Account.Commands.UpdateAccountPassword;

public sealed class UpdateAccountPasswordCommandHandler(
    IAccountRepository accountRepository,
    IUnitOfWork unitOfWork,
    IPasswordHasher passwordHasher)
    : IRequestHandler<UpdateAccountPasswordCommand, bool>
{
    public async Task<bool> Handle(UpdateAccountPasswordCommand request, CancellationToken ct)
    {
        var account = await accountRepository.GetByIdAsync(request.AccountId, ct);
        if (account is null)
            return false;

        string passwordHash = passwordHasher.Create(request.NewPassword);

        account.UpdatePassword(passwordHash);
        await unitOfWork.SaveChangesAsync(ct);
        return true;
    }
}
