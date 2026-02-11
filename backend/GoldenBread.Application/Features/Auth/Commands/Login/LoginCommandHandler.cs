using GoldenBread.Application.Common.Abstractions.Data;
using GoldenBread.Application.Common.Abstractions.Services;
using GoldenBread.Application.Features.Auth.Commands.Login;
using GoldenBread.Domain.Entities;

namespace GoldenBread.Application.Features.Auth.Commands.Login;

public sealed class LoginCompanyCommandHandler(
    IGoldenBreadContext context,
    ISessionService sessionService,
    IPasswordHasher passwordHasher)
    : IRequestHandler<LoginCommand, AuthResponse?>
{
    public async Task<AuthResponse?> Handle(
        LoginCommand command,
        CancellationToken cancellationToken)
    {
        Account? account = await context.Accounts
            .FirstOrDefaultAsync(c =>
                c.Email == command.Email,
                cancellationToken);

        if (account == null ||
            !passwordHasher.Verify(command.Password, account.PasswordHash))
        {
            return null;
        }

        (string session, DateTime sessionExpAt) = sessionService.Create();

        return new AuthResponse(
            account.AccountId,
            session,
            sessionExpAt,
            account.VerificationStatus);
    }
}