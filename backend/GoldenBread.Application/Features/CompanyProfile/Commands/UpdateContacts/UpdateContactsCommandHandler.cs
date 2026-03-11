using GoldenBread.Application.Abstractions.Data;
using GoldenBread.Application.Abstractions.Services;

namespace GoldenBread.Application.Features.CompanyProfile.Commands.UpdateContacts;

public sealed class UpdateCompanyContactsCommandHandler(
    IGoldenBreadContext context,
    ICurrentAccountContext accountContext,
    IUniquenessChecker checker) :   
    IRequestHandler<UpdateContactsCommand, Unit>    
{
    public async Task<Unit> Handle(
        UpdateContactsCommand command,
        CancellationToken ct)
    {
        var account = await accountContext.GetAccountAsync(ct);
        var company = account.Company;

        await checker.CompanyPhoneMustBeUniqueAsync(company.Phone, company.CompanyId, ct);
        await checker.CompanyAddressMustBeUniqueAsync(company.Address, company.CompanyId, ct);

        company.UpdateContacts(command.Phone, command.Address);

        await context.SaveChangesAsync(ct);

        return Unit.Value;
    }
}
