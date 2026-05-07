using GoldenBread.Application.Abstractions.Data;
using GoldenBread.Application.Abstractions.Data.Repositories;
using GoldenBread.Application.Common.Exceptions;

namespace GoldenBread.Application.Features.Companies.Commands.UpdateCompany;

public sealed class UpdateCompanyCommandHandler(
    ICompanyRepository companyRepository,
    IUnitOfWork unitOfWork)
    : IRequestHandler<UpdateCompanyCommand, bool>
{
    public async Task<bool> Handle(UpdateCompanyCommand request, CancellationToken ct)
    {
        var dto = request.CompanyDto;

        if (await companyRepository.ExistsByInnAsync(dto.Inn, dto.CompanyId, ct))
            throw new DuplicateEntityException(nameof(dto.Inn));
        else if (await companyRepository.ExistsByOgrnAsync(dto.Ogrn, dto.CompanyId, ct))
            throw new DuplicateEntityException(nameof(dto.Ogrn));
        else if (await companyRepository.ExistsByPhoneAsync(dto.Phone, dto.CompanyId, ct))
            throw new DuplicateEntityException(nameof(dto.Phone));
        else if (await companyRepository.ExistsByNameAsync(dto.Name, dto.CompanyId, ct))
            throw new DuplicateEntityException(nameof(dto.Name));

        var company = await companyRepository.GetByIdAsync(dto.CompanyId, ct);

        if (company is null)
            return false;

        company.Update(dto.Name, dto.Inn, dto.Ogrn, dto.Phone, dto.Address);

        await unitOfWork.SaveChangesAsync(ct);
        return true;
    }
}
