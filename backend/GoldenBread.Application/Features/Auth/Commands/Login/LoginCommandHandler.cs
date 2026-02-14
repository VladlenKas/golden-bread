using GoldenBread.Application.Common.Abstractions.Data;
using GoldenBread.Application.Common.Abstractions.Services;

namespace GoldenBread.Application.Features.Auth.Commands.Login;

public sealed class LoginCompanyCommandHandler(
    IGoldenBreadContext context,
    ICookieService cookieService,
    ISessionService sessionService,
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

        (string session, DateTime sessionExpAt) = sessionService.Create();
        account.SetSession(session, sessionExpAt);

        await cookieService.SignInAsync(session);

        return new AuthResponse(
            account.AccountId,
            account.AccountType,
            account.VerificationStatus);
    }
}