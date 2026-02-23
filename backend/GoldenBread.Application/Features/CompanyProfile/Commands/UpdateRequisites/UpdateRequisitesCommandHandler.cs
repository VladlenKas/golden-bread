using GoldenBread.Application.Services;

namespace GoldenBread.Application.Features.CompanyProfile.Commands.UpdateRequisites;

public sealed class UpdateCompanyRequisitesCommandHandler(
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

        return Unit.Value;
    }
}
