using GoldenBread.Application.Common.Abstractions;
using GoldenBread.Application.Common.Abstractions.Data;
using GoldenBread.Application.Common.Abstractions.Services;
using GoldenBread.Contracts.Responses;
using GoldenBread.Domain.Entities;
using GoldenBread.Domain.Enums;

namespace GoldenBread.Application.Features.Auth.Commands.LoginUser;

public class LoginUserCommandHandler(
    IGoldenBreadContext context,
    ISessionService sessionService,
    IPasswordHasher passwordHasher) 
    : IRequestHandler<LoginUserCommand, LoginUserResponse?>
{
    public async Task<LoginUserResponse?> Handle(
        LoginUserCommand command,
        CancellationToken cancellationToken)
    {
        Account? account = await context.Accounts
            .FirstOrDefaultAsync(c =>
                c.Email == command.Email &&
                c.AccountType == AccountType.User,
                cancellationToken);

        if (account == null ||
            !passwordHasher.Verify(command.Password, account.PasswordHash))
        {
            return null;
        }

        (string session, DateTime sessionExpAt) = sessionService.Create();

        return new LoginUserResponse
        {
            Id = account.AccountId,
            Fullname = account.User.GetFullName(),
            Role = account.User.Role.ToString(),
            Session = session,
            SessionExpiresAt = sessionExpAt,
            AccountStatus = account.VerificationStatus.ToString()
        };
    }
}
