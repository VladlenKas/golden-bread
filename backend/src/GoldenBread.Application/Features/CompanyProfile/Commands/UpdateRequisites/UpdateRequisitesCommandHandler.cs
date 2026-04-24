using GoldenBread.Application.Abstractions.Data;
using GoldenBread.Application.Abstractions.Data.Repositories;
using GoldenBread.Application.Abstractions.Services;
using GoldenBread.Application.Common.Constants;
using GoldenBread.Application.Common.Exceptions;

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
        var company = account.GetCompany();

        if (await companyRepository.ExistsByNameAsync(command.Name, company.CompanyId, ct))
            throw new DuplicateEntityException(nameof(command.Name));

        if (await companyRepository.ExistsByInnAsync(command.Inn, company.CompanyId, ct))
            throw new DuplicateEntityException(nameof(command.Inn));

        if (await companyRepository.ExistsByOgrnAsync(command.Ogrn, company.CompanyId, ct))
            throw new DuplicateEntityException(nameof(command.Ogrn));

        company.UpdateRequisites(command.Name, command.Inn, command.Ogrn);
        account.SetPendingVerification();

        await unitOfWork.SaveChangesAsync(ct);

        await cookieService.SignOutAsync();

        return Unit.Value;
    }
}
