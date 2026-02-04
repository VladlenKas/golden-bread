using GoldenBread.Application.Common.Abstractions.Repositories;
using GoldenBread.Application.Common.Abstractions.Services;
using GoldenBread.Contracts.Responses;
using GoldenBread.Domain.Entities;
using GoldenBread.Domain.Enums;

namespace GoldenBread.Application.Features.Auth.Commands.RegisterCompany;

public class RegisterCompanyCommandHandler(
    IAccountRepository accountRepository,
    ICompanyRepository companyRepository,
    ISessionService sessionService,
    IPasswordHasher passwordHasher)
    : IRequestHandler<RegisterCompanyCommand, RegisterCompanyResponse>
{
    public async Task<RegisterCompanyResponse> Handle(
        RegisterCompanyCommand command,
        CancellationToken cancellationToken)
    {
        (string session, DateTime sessionExpAt) = sessionService.GenerateSession();
        string passwordHash = passwordHasher.GeneratePassword(command.Password);

        var account = Account.Create(
            command.Email,
            passwordHash,
            AccountType.Company,
            session,
            sessionExpAt
        );

        await accountRepository.AddAsync(account, cancellationToken);

        var company = Company.Create(
            command.Name,
            command.Inn,
            command.Ogrn,
            account.AccountId
        );

        await companyRepository.AddAsync(company, cancellationToken);

        return new RegisterCompanyResponse
        {
            Id = account.AccountId,
            Session = session,
            SessionExpiresAt = sessionExpAt,
            AccountStatus = account.VerificationStatus.ToString(),
        };
    }
}
