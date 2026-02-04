using GoldenBread.Application.Common.Abstractions.Repositories;
using GoldenBread.Application.Common.Abstractions.Services;
using GoldenBread.Contracts.Responses;
using GoldenBread.Domain.Entities;
using GoldenBread.Domain.Enums;

namespace GoldenBread.Application.Features.Auth.Commands.LoginCompany;

public class LoginCompanyCommandHandler(
    IAccountRepository accountRepository,
    ISessionService sessionService,
    IPasswordHasher passwordHasher)
    : IRequestHandler<LoginCompanyCommand, LoginCompanyResponse?>
{
    public async Task<LoginCompanyResponse?> Handle(
        LoginCompanyCommand command, 
        CancellationToken cancellationToken)
    {
        Account? account = await accountRepository.GetByEmailAsync(command.Email, cancellationToken);

        if (account == null ||
            account.AccountType != AccountType.Company ||
            !passwordHasher.VerifyPassword(command.Password, account.PasswordHash))
        {
            return null;
        }

        (string session, DateTime sessionExpAt) = sessionService.GenerateSession();

        return new LoginCompanyResponse
        {
            Id = account.AccountId,
            Name = account.Company.Name,
            Session = session,
            SessionExpiresAt = sessionExpAt,
            AccountStatus = account.VerificationStatus.ToString(),
        };
    }
}
