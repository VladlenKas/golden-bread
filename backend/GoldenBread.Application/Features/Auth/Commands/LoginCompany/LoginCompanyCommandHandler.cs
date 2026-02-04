using GoldenBread.Application.Common.Abstractions;
using GoldenBread.Application.Common.Abstractions.Data;
using GoldenBread.Application.Common.Abstractions.Services;
using GoldenBread.Contracts.Responses;
using GoldenBread.Domain.Entities;
using GoldenBread.Domain.Enums;

namespace GoldenBread.Application.Features.Auth.Commands.LoginCompany;

public class LoginCompanyCommandHandler(
    IGoldenBreadContext context,
    ISessionService sessionService,
    IPasswordHasher passwordHasher)
    : IRequestHandler<LoginCompanyCommand, LoginCompanyResponse?>
{
    public async Task<LoginCompanyResponse?> Handle(
        LoginCompanyCommand command, 
        CancellationToken cancellationToken)
    {
        Account? account = await context.Accounts
            .FirstOrDefaultAsync(c => 
                c.Email == command.Email &&
                c.AccountType == AccountType.Company,
                cancellationToken);

        if (account == null || 
            !passwordHasher.Verify(command.Password, account.PasswordHash))
        {
            return null;
        }

        (string session, DateTime sessionExpAt) = sessionService.Create();

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
