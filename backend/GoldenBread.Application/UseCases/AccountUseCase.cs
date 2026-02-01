using FluentValidation;
using GoldenBread.Application.Interfaces;
using GoldenBread.Application.Repositories;
using GoldenBread.Contracts.Requests;
using GoldenBread.Contracts.Responses;
using GoldenBread.Domain.Entities;
using GoldenBread.Domain.Enums;
using BCryptNet = BCrypt.Net.BCrypt;

namespace GoldenBread.Application.UseCases;

public class AccountUseCase(
    IAccountRepository accountRepository,
    ICompanyRepository companyRepository,
    IValidator<RegisterCompanyRequest> validator) : IAccountUseCase
{
    public async Task<LoginUserResponse?> LoginUserAsync(LoginRequest request)
    {
        Account? account = await accountRepository.GetByEmailAsync(request.Email);

        if (account == null ||
            account.AccountType != AccountType.User ||
            !BCryptNet.Verify(request.Password, account.Password))
        {
            return null;
        }

        var fullname = $"{account.User.Lastname} {account.User.Firstname} {account.User.Patronymic}".Trim();
        var session = $"{Guid.NewGuid()}@{DateTime.UtcNow:O}";
        var sessionExpAt = DateTime.UtcNow.AddDays(7);

        return new LoginUserResponse
        {
            Id = account.AccountId,
            Fullname = fullname,
            Role = account.User.Role.ToString(),
            Session = session,
            SessionExpiresAt = sessionExpAt,
            AccountStatus = account.VerificationStatus.ToString(),
        };
    }

    public async Task<LoginCompanyResponse?> LoginCompanyAsync(LoginRequest request)
    {
        Account? account = await accountRepository.GetByEmailAsync(request.Email);

        if (account == null ||
            account.AccountType != AccountType.Company ||
            !BCryptNet.Verify(request.Password, account.Password))
        {
            return null;
        }

        var session = $"{Guid.NewGuid()}@{DateTime.UtcNow:O}";
        var sessionExpAt = DateTime.UtcNow.AddDays(7);

        return new LoginCompanyResponse
        {
            Id = account.AccountId,
            Name = account.Company.Name,
            Session = session,
            SessionExpiresAt = sessionExpAt,
            AccountStatus = account.VerificationStatus.ToString(),
        };
    }

    public async Task<RegisterCompanyResponse> RegisterCompanyAsync(RegisterCompanyRequest request)
    {
        await validator.ValidateAsync(request);

        var session = $"{Guid.NewGuid()}@{DateTime.UtcNow:O}";
        var sessionExpAt = DateTime.UtcNow.AddDays(7);
        var password = BCryptNet.HashPassword(request.Password);

        var account = new Account()
        {
            Email = request.Email,
            Password = password,
            AccountType = AccountType.Company,
            Session = session,
            SessionExpiresAt = sessionExpAt,
        };

        await accountRepository.AddAsync(account);

        var company = new Company()
        {
            Name = request.Name,
            Inn = request.Inn,
            Ogrn = request.Ogrn,
            AccountId = account.AccountId,
        };

        await companyRepository.AddAsync(company);

        var response = new RegisterCompanyResponse
        {
            Id = account.AccountId,
            Session = session,
            SessionExpiresAt = sessionExpAt,
            AccountStatus = account.VerificationStatus.ToString(),
        };

        return response;
    }
}
