using GoldenBread.Application.Abstractions.Data;
using GoldenBread.Application.Abstractions.Enums;
using GoldenBread.Application.Features.Auth.Dtos;
using GoldenBread.Application.Services;
using GoldenBread.Domain.Enums;

namespace GoldenBread.Application.Features.Auth.Commands.Login;

public sealed class LoginCompanyCommandHandler(
    IGoldenBreadContext context,
    ICookieService cookieService,
    IPasswordHasher passwordHasher) : 
    IRequestHandler<LoginCommand, AuthResponse>
{
    public async Task<AuthResponse> Handle(
        LoginCommand command,
        CancellationToken cancellationToken)
    {
        var account = await context.Accounts
            .FirstOrDefaultAsync(c =>
                c.Email == command.Email,
                cancellationToken);

        if (account == null ||
            !passwordHasher.Verify(command.Password, account.PasswordHash) ||
            !HasAccess(account.AccountType, command.PortalType))
            throw new UnauthorizedAccessException();

        account.SetSession();

        await cookieService.SignInAsync(account.Session!);

        return new AuthResponse(
            account.AccountId,
            account.VerificationStatus);
    }

    private bool HasAccess(AccountType accountType, PortalType portal)
        => (accountType, portal) switch
        {
            (AccountType.Company, PortalType.Company) => true,
            (AccountType.User, PortalType.User) => true,
            _ => false,
        };
}