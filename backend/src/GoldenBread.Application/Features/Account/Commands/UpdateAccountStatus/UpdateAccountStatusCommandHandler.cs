using GoldenBread.Application.Abstractions.Data;
using GoldenBread.Application.Abstractions.Data.Repositories;

namespace GoldenBread.Application.Features.Account.Commands.UpdateAccountStatus;

public sealed class UpdateAccountStatusCommandHandler(
    IAccountRepository accountRepository,
    IUnitOfWork unitOfWork)
    : IRequestHandler<UpdateAccountStatusCommand, bool>
{
    public async Task<bool> Handle(UpdateAccountStatusCommand request, CancellationToken ct)
    {
        var account = await accountRepository.GetByIdAsync(request.AccountId, ct);
        if (account is null)
            return false;

        account.UpdateStatus(request.Status);
        await unitOfWork.SaveChangesAsync(ct);
        return true;
    }
}
