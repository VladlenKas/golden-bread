using GoldenBread.Application.Common.Abstractions.Repositories;
using GoldenBread.Application.Common.Abstractions.Services;
using GoldenBread.Contracts.Responses;
using GoldenBread.Domain.Entities;
using GoldenBread.Domain.Enums;
using MediatR;

namespace GoldenBread.Application.Features.Auth.Commands.RegisterCompany;

public class RegisterCompanyCommandHandler(
    IAccountRepository accountRepository,
    ICompanyRepository companyRepository,
    ISessionService sessionService,
    IPasswordHasher passwordHasher)
    : IRequestHandler<RegisterCompanyCommand, RegisterCompanyResponse?>
{
    public async Task<RegisterCompanyResponse?> Handle(
        RegisterCompanyCommand command, 
        CancellationToken cancellationToken)
    {
        string session = sessionService.GenerateSessionId();
        DateTime sessionExpAt = sessionService.GenerateSessionExpiry();
        string password = passwordHasher.GeneratePassword(command.Password);

        var account = new Account()
        {
            Email = command.Email,
            Password = password,
            AccountType = AccountType.Company,
            Session = session,
            SessionExpiresAt = sessionExpAt,
        };

        await accountRepository.AddAsync(account, cancellationToken);

        var company = new Company()
        {
            Name = command.Name,
            Inn = command.Inn,
            Ogrn = command.Ogrn,
            AccountId = account.AccountId,
        };

        await companyRepository.AddAsync(company, cancellationToken);

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
