using GoldenBread.Application.Common.Abstractions.Repositories;
using GoldenBread.Contracts.Responses;
using GoldenBread.Domain.Entities;
using GoldenBread.Domain.Enums;
using MediatR;
using BCryptNet = BCrypt.Net.BCrypt;

namespace GoldenBread.Application.Features.Auth.Commands.LoginUser;

public class LoginUserCommandHandler(IAccountRepository accountRepository) 
    : IRequestHandler<LoginUserCommand, LoginUserResponse?>
{
    public async Task<LoginUserResponse?> Handle(
        LoginUserCommand request,
        CancellationToken cancellationToken)
    {
        Account? account = await accountRepository.GetByEmailAsync(request.Email);

        if (account == null ||
            account.AccountType != AccountType.User ||
            !BCryptNet.Verify(request.Password, account.Password))
        {
            return null;
        }

        var fullname = $"{account.User.Lastname} {account.User.Firstname} {account.User.Patronymic}".Trim();
        var session = $"{Guid.NewGuid()}@{DateTime.UtcNow:O}";
        var sessionExpAt = DateTime.UtcNow.AddDays(7);

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
