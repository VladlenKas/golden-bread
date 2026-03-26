using GoldenBread.Application.Abstractions.Data;
using GoldenBread.Application.Abstractions.Data.Repositories;
using GoldenBread.Application.Abstractions.Services;
using GoldenBread.Application.Common.Constants;
using GoldenBread.Application.Common.Exceptions;

namespace GoldenBread.Application.Features.CompanyProfile.Commands.UpdateContacts;

public sealed class UpdateCompanyContactsCommandHandler(
    IUnitOfWork unitOfWork,
    ICompanyRepository companyRepository,
    ICurrentAccountContext accountContext) :   
    IRequestHandler<UpdateContactsCommand, Unit>    
{
    public async Task<Unit> Handle(
        UpdateContactsCommand command,
        CancellationToken ct)
    {
        var account = await accountContext.GetAccountAsync(ct);
        var company = account.GetCompany();

        if (await companyRepository.ExistsByPhoneAsync(command.Phone, account.AccountId, ct))
            throw new DuplicateEntityException(nameof(command.Phone));

        if (await companyRepository.ExistsByAddressAsync(command.Address, account.AccountId, ct))
            throw new DuplicateEntityException(nameof(command.Address));

        company.UpdateContacts(command.Phone, command.Address);

        await unitOfWork.SaveChangesAsync(ct);

        return Unit.Value;
    }
}
