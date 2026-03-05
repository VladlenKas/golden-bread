using GoldenBread.Application.Abstractions.Data;
using GoldenBread.Application.Abstractions.Services;

namespace GoldenBread.Application.Features.CompanyProfile.Commands.UpdateRequisites;

public sealed class UpdateCompanyRequisitesCommandHandler(
    IGoldenBreadContext context,
    ICurrentAccountContext accountContext,
    ICookieService cookieService,
    IUniquenessChecker checker) :
    IRequestHandler<UpdateRequisitesCommand, Unit>
{
    public async Task<Unit> Handle(
        UpdateRequisitesCommand command,
        CancellationToken cancellationToken)
    {
        var account = await accountContext.GetAccountAsync(cancellationToken);
        var company = account.Company;

        await checker.CompanyNameMustBeUniqueAsync(command.Name, company.CompanyId, cancellationToken);
        await checker.CompanyInnMustBeUniqueAsync(command.Inn, company.CompanyId, cancellationToken);
        await checker.CompanyOgrnMustBeUniqueAsync(command.Ogrn, company.CompanyId, cancellationToken);

        company.UpdateRequisites(command.Name, command.Inn, command.Ogrn);
        account.SetPendingVerification();

        await cookieService.SignOutAsync();

        await context.SaveChangesAsync(cancellationToken);

        return Unit.Value;
    }
}
