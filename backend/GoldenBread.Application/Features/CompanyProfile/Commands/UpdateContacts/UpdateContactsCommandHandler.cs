using GoldenBread.Application.Abstractions.Data;
using GoldenBread.Application.Abstractions.Repositories;
using GoldenBread.Application.Abstractions.Services;
using GoldenBread.Application.Common.Exceptions.Domain;

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
        var company = account.Company ?? 
            throw new AccountHasNoCompanyException(account.AccountId);

        if (await companyRepository.ExistsByPhoneAsync(command.Phone, account.AccountId, ct))
            throw new PhoneDuplicateException();

        if (await companyRepository.ExistsByAddressAsync(command.Address, account.AccountId, ct))
            throw new AddressDuplicateException();

        company.UpdateContacts(command.Phone, command.Address);

        await unitOfWork.SaveChangesAsync(ct);

        return Unit.Value;
    }
}
