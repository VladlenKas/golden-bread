using GoldenBread.Application.Common.Abstractions.Repositories;
using GoldenBread.Contracts.Responses;
using GoldenBread.Domain.Entities;
using GoldenBread.Domain.Enums;
using MediatR;
using BCryptNet = BCrypt.Net.BCrypt;

namespace GoldenBread.Application.Features.Auth.Commands.RegisterCompany;

public class RegisterCompanyCommandHandler(
    IAccountRepository accountRepository,
    ICompanyRepository companyRepository)
    : IRequestHandler<RegisterCompanyCommand, RegisterCompanyResponse?>
{
    public async Task<RegisterCompanyResponse?> Handle(
        RegisterCompanyCommand request, 
        CancellationToken cancellationToken)
    {
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
