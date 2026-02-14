using GoldenBread.Application.Common.Abstractions.Data;
using GoldenBread.Application.Common.Abstractions.Services;
using GoldenBread.Domain.Entities;
using GoldenBread.Domain.Enums;

namespace GoldenBread.Application.Features.Auth.Commands.RegisterCompany;

public sealed class RegisterCompanyCommandHandler(
    IGoldenBreadContext context,
    ICookieService cookieService,
    IPasswordHasher passwordHasher)
    : IRequestHandler<RegisterCompanyCommand, AuthResponse>
{
    public async Task<AuthResponse> Handle(
        RegisterCompanyCommand command,
        CancellationToken cancellationToken)
    {
        string passwordHash = passwordHasher.Create(command.Password);

        var account = Account.Create(
            command.Email,
            passwordHash,
            AccountType.Company
        );

        account.SetSession();

        var company = Company.Create(
            command.Name,
            command.Inn,
            command.Ogrn,
            account
        );

        await context.Accounts.AddAsync(account, cancellationToken);
        await context.Companies.AddAsync(company, cancellationToken);

        await cookieService.SignInAsync(account.Session!);

        return new AuthResponse(
            account.AccountId,
            account.AccountType,
            account.VerificationStatus);
    }
}
