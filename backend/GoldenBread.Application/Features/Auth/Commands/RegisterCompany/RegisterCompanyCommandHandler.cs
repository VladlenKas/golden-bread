using GoldenBread.Application.Abstractions.Data;
using GoldenBread.Application.Abstractions.Repositories;
using GoldenBread.Application.Abstractions.Services;
using GoldenBread.Application.Features.Auth.Dtos;
using GoldenBread.Domain.Entities;
using GoldenBread.Domain.Enums;

namespace GoldenBread.Application.Features.Auth.Commands.RegisterCompany;

public sealed class RegisterCompanyCommandHandler(
    IUnitOfWork unitOfWork,
    ICompanyRepository companyRepository,
    IAccountRepository accountRepository,
    ICookieService cookieService,
    IPasswordHasher passwordHasher) :
    IRequestHandler<RegisterCompanyCommand, AuthResponse>
{
    public async Task<AuthResponse> Handle(
        RegisterCompanyCommand command,
        CancellationToken ct)
    {
        await unitOfWork.BeginTransactionAsync(ct);

        try
        {
            string passwordHash = passwordHasher.Create(command.Password);

            var account = Account.Create(
                command.Email,
                passwordHash,
                AccountType.Company
            );

            account.SetSession();

            await accountRepository.AddAsync(account, ct);
            await unitOfWork.SaveChangesAsync(ct);

            var company = Company.Create(
                account.AccountId,
                command.Name,
                command.Inn,
                command.Ogrn
            );

            await companyRepository.AddAsync(company, ct);
            await unitOfWork.CommitAsync(ct);

            await cookieService.SignInAsync(account.Session!);

            return new AuthResponse(
                account.AccountId,
                account.VerificationStatus);
        }
        catch
        {
            await unitOfWork.RollbackAsync(ct);
            throw;
        }
    }
}
