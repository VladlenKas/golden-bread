using GoldenBread.Application.Common.Abstractions.Repositories;
using GoldenBread.Contracts.Responses;
using GoldenBread.Domain.Entities;
using GoldenBread.Domain.Enums;
using MediatR;
using BCryptNet = BCrypt.Net.BCrypt;

namespace GoldenBread.Application.Features.Auth.Commands.LoginCompany;

public class LoginCompanyCommandHandler(IAccountRepository accountRepository)
    : IRequestHandler<LoginCompanyCommand, LoginCompanyResponse?>
{
    public async Task<LoginCompanyResponse?> Handle(
        LoginCompanyCommand request, 
        CancellationToken cancellationToken)
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
}
