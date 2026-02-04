using GoldenBread.Application.Common.Abstractions;
using GoldenBread.Application.Common.Abstractions.Data;
using GoldenBread.Application.Common.Abstractions.Services;
using GoldenBread.Contracts.Responses;
using GoldenBread.Domain.Entities;
using GoldenBread.Domain.Enums;

namespace GoldenBread.Application.Features.Auth.Commands.RegisterCompany;

public class RegisterCompanyCommandHandler(
    IGoldenBreadContext context,
    ISessionService sessionService,
    IPasswordHasher passwordHasher)
    : IRequestHandler<RegisterCompanyCommand, RegisterCompanyResponse>
{
    public async Task<RegisterCompanyResponse> Handle(
        RegisterCompanyCommand command,
        CancellationToken cancellationToken)
    {
        (string session, DateTime sessionExpAt) = sessionService.Create();
        string passwordHash = passwordHasher.Create(command.Password);

        var account = Account.Create(
            command.Email,
            passwordHash,
            AccountType.Company,
            session,
            sessionExpAt
        );

        await context.Accounts.AddAsync(account, cancellationToken);

        var company = Company.Create(
            command.Name,
            command.Inn,
            command.Ogrn,
            account
        );

        await context.Companies.AddAsync(company, cancellationToken);

        return new RegisterCompanyResponse
        {
            Id = account.AccountId,
            Session = session,
            SessionExpiresAt = sessionExpAt,
            AccountStatus = account.VerificationStatus.ToString(),
        };
    }
}
