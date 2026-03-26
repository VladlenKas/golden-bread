using GoldenBread.Application.Abstractions.Data;
using GoldenBread.Application.Abstractions.Data.Repositories;
using GoldenBread.Application.Abstractions.Services;
using GoldenBread.Application.Common.Exceptions.Domain;

namespace GoldenBread.Application.Features.CompanyProfile.Commands.UpdateRequisites;

public sealed class UpdateCompanyRequisitesCommandHandler(
    IUnitOfWork unitOfWork,
    ICompanyRepository companyRepository,
    ICurrentAccountContext accountContext,
    ICookieService cookieService) :
    IRequestHandler<UpdateRequisitesCommand, Unit>
{
    public async Task<Unit> Handle(
        UpdateRequisitesCommand command,
        CancellationToken ct)
    {
        var account = await accountContext.GetAccountAsync(ct);
        var company = account.Company ?? 
            throw new AccountHasNoCompanyException(account.AccountId);

        if (await companyRepository.ExistsByNameAsync(command.Name, account.AccountId, ct))
            throw new NameDuplicateException();

        if (await companyRepository.ExistsByInnAsync(command.Inn, account.AccountId, ct))
            throw new InnDuplicateException();

        if (await companyRepository.ExistsByOgrnAsync(command.Ogrn, account.AccountId, ct))
            throw new OgrnDuplicateException();

        company.UpdateRequisites(command.Name, command.Inn, command.Ogrn);
        account.SetPendingVerification();

        await unitOfWork.SaveChangesAsync(ct);

        await cookieService.SignOutAsync();

        return Unit.Value;
    }
}
