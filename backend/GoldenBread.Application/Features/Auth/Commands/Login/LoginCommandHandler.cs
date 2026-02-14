using GoldenBread.Application.Abstractions.Data;
using GoldenBread.Application.Services;

namespace GoldenBread.Application.Features.Auth.Commands.Login;

public sealed class LoginCompanyCommandHandler(
    IGoldenBreadContext context,
    ICookieService cookieService,
    IPasswordHasher passwordHasher)
    : IRequestHandler<LoginCommand, AuthResponse>
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
            !passwordHasher.Verify(command.Password, account.PasswordHash))
            throw new UnauthorizedAccessException();

        account.SetSession();

        await cookieService.SignInAsync(account.Session!);

        return new AuthResponse(
            account.AccountId,
            account.AccountType,
            account.VerificationStatus);
    }
}