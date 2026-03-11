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
        CancellationToken ct)
    {
        var account = await accountContext.GetAccountAsync(ct);
        var company = account.Company;

        await checker.CompanyNameMustBeUniqueAsync(command.Name, company.CompanyId, ct);
        await checker.CompanyInnMustBeUniqueAsync(command.Inn, company.CompanyId, ct);
        await checker.CompanyOgrnMustBeUniqueAsync(command.Ogrn, company.CompanyId, ct);

        company.UpdateRequisites(command.Name, command.Inn, command.Ogrn);
        account.SetPendingVerification();

        await cookieService.SignOutAsync();

        await context.SaveChangesAsync(ct);

        return Unit.Value;
    }
}
