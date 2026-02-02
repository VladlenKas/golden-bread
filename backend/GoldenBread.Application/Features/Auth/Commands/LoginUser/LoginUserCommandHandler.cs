using GoldenBread.Application.Common.Abstractions.Repositories;
using GoldenBread.Application.Common.Abstractions.Services;
using GoldenBread.Contracts.Responses;
using GoldenBread.Domain.Entities;
using GoldenBread.Domain.Enums;
using MediatR;
using BCryptNet = BCrypt.Net.BCrypt;

namespace GoldenBread.Application.Features.Auth.Commands.LoginUser;

public class LoginUserCommandHandler(
    IAccountRepository accountRepository,
    ISessionService sessionService,
    IPasswordHasher passwordHasher) 
    : IRequestHandler<LoginUserCommand, LoginUserResponse?>
{
    public async Task<LoginUserResponse?> Handle(
        LoginUserCommand command,
        CancellationToken cancellationToken)
    {
        Account? account = await accountRepository.GetByEmailAsync(command.Email, cancellationToken);

        if (account == null ||
            account.AccountType != AccountType.User ||
            !passwordHasher.VerifyPassword(command.Password, account.Password))
        {
            return null;
        }

        var fullname = $"{account.User.Lastname} {account.User.Firstname} {account.User.Patronymic}".Trim();
        string session = sessionService.GenerateSessionId();
        DateTime sessionExpAt = sessionService.GenerateSessionExpiry();

        return new LoginUserResponse
        {
            Id = account.AccountId,
            Fullname = fullname,
            Role = account.User.Role.ToString(),
            Session = session,
            SessionExpiresAt = sessionExpAt,
            AccountStatus = account.VerificationStatus.ToString()
        };
    }
}
