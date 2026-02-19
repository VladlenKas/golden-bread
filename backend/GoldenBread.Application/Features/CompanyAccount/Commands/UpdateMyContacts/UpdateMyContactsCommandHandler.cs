using GoldenBread.Application.Services;

namespace GoldenBread.Application.Features.CompanyAccount.Commands.UpdateMyContacts;

public sealed class UpdateCompanyContactsCommandHandler(
    ICurrentAccountContext accountContext,
    IUniquenessChecker checker) :
    IRequestHandler<UpdateMyContactsCommand, Unit>
{
    public async Task<Unit> Handle(
        UpdateMyContactsCommand command,
        CancellationToken cancellationToken)
    {
        var account = await accountContext.GetAccountAsync(cancellationToken);
        var company = account.Company;

        await checker.CompanyPhoneMustBeUniqueAsync(company.Phone, company.CompanyId, cancellationToken);
        await checker.CompanyAddressMustBeUniqueAsync(company.Address, company.CompanyId, cancellationToken);

        company.UpdateContacts(command.Phone, command.Address);

        return Unit.Value;
    }
}
