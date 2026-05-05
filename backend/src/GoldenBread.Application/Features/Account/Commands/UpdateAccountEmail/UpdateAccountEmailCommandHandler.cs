using GoldenBread.Application.Abstractions.Data;
using GoldenBread.Application.Abstractions.Data.Repositories;

namespace GoldenBread.Application.Features.Account.Commands.UpdateAccountEmail;

public sealed class UpdateAccountEmailCommandHandler(
    IAccountRepository accountRepository,
    IUnitOfWork unitOfWork)
    : IRequestHandler<UpdateAccountEmailCommand, bool>
{
    public async Task<bool> Handle(UpdateAccountEmailCommand request, CancellationToken ct)
    {
        var account = await accountRepository.GetByIdAsync(request.AccountId, ct);
        if (account is null)
            return false;

        account.UpdateEmail(request.NewEmail);
        await unitOfWork.SaveChangesAsync(ct);
        return true;
    }
}
