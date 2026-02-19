using GoldenBread.Application.Abstractions.Data;
using GoldenBread.Application.Features.Auth.Dtos;
using GoldenBread.Application.Services;
using GoldenBread.Domain.Entities;
using GoldenBread.Domain.Enums;

namespace GoldenBread.Application.Features.Auth.Commands.RegisterCompany;

public sealed class RegisterCompanyCommandHandler(
    IGoldenBreadContext context,
    IUniquenessChecker checker,
    ICookieService cookieService,
    IPasswordHasher passwordHasher) : 
    IRequestHandler<RegisterCompanyCommand, AuthResponse>
{
    public async Task<AuthResponse> Handle(
        RegisterCompanyCommand command,
        CancellationToken cancellationToken)
    {
        await checker.EmailMustBeUniqueAsync(command.Email, ct: cancellationToken);
        await checker.CompanyNameMustBeUniqueAsync(command.Name, ct: cancellationToken);
        await checker.CompanyInnMustBeUniqueAsync(command.Inn, ct: cancellationToken);
        await checker.CompanyOgrnMustBeUniqueAsync(command.Ogrn, ct: cancellationToken);

        string passwordHash = passwordHasher.Create(command.Password);

        var account = Account.Create(
            command.Email,
            passwordHash,
            AccountType.Company
        );

        account.SetSession();

        var company = Entities.Company.Create(
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
            account.VerificationStatus);
    }
}
