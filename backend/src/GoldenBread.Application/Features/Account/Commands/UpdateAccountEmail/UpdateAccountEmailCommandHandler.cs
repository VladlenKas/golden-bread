using GoldenBread.Application.Abstractions.Data;
using GoldenBread.Application.Abstractions.Data.Repositories;
using GoldenBread.Application.Common.Exceptions;
using static Microsoft.EntityFrameworkCore.DbLoggerCategory.Database;

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

        if (await accountRepository.ExistsByEmailAsync(request.NewEmail, account.AccountId, ct))
            throw new DuplicateEntityException(nameof(request.NewEmail));

        account.ClearSession();

        account.UpdateEmail(request.NewEmail);
        await unitOfWork.SaveChangesAsync(ct);
        return true;
    }
}
