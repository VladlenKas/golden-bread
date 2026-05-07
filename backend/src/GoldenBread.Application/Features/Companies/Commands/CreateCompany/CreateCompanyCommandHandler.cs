using GoldenBread.Application.Abstractions.Data;
using GoldenBread.Application.Abstractions.Data.Repositories;
using GoldenBread.Application.Abstractions.Services;
using GoldenBread.Application.Common.Exceptions;
using GoldenBread.Domain.Entities;
using GoldenBread.Domain.Enums;

namespace GoldenBread.Application.Features.Companies.Commands.CreateCompany;

public sealed class CreateCompanyCommandHandler(
    ICompanyRepository companyRepository,
    IAccountRepository accountRepository,
    IUnitOfWork unitOfWork,
    IPasswordHasher passwordHasher)
    : IRequestHandler<CreateCompanyCommand, int>
{
    public async Task<int> Handle(CreateCompanyCommand request, CancellationToken ct)
    {
        await unitOfWork.BeginTransactionAsync(ct);

        try
        {
            var dto = request.CompanyDto;

            if (await accountRepository.ExistsByEmailAsync(request.Email, null, ct))
                throw new DuplicateEntityException(nameof(request.Email));
            else if (await companyRepository.ExistsByInnAsync(dto.Inn, null, ct))
                throw new DuplicateEntityException(nameof(dto.Inn));
            else if (await companyRepository.ExistsByOgrnAsync(dto.Ogrn, null, ct))
                throw new DuplicateEntityException(nameof(dto.Ogrn));
            else if (await companyRepository.ExistsByPhoneAsync(dto.Phone, null, ct))
                throw new DuplicateEntityException(nameof(dto.Phone));
            else if (await companyRepository.ExistsByNameAsync(dto.Name, null, ct))
                throw new DuplicateEntityException(nameof(dto.Name));

            string passwordHash = passwordHasher.Create(request.Password);

            var account = DbEntities.Account.Create(
                0,
                request.Email,
                passwordHash,
                AccountType.Company);

            await accountRepository.AddAsync(account, ct);
            await unitOfWork.SaveChangesAsync(ct);

            var company = Company.Create(
                0,
                account.AccountId,
                dto.Name,
                dto.Inn,
                dto.Ogrn,
                dto.Phone,
                dto.Address);

            await companyRepository.AddAsync(company, ct);
            await unitOfWork.CommitAsync(ct);

            return company.CompanyId;
        }
        catch
        {
            await unitOfWork.RollbackAsync(ct);
            throw;
        }
    }
}
