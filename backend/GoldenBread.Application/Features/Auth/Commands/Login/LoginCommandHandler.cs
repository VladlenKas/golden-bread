using GoldenBread.Application.Abstractions.Data;
using GoldenBread.Application.Abstractions.Data.Repositories;
using GoldenBread.Application.Abstractions.Services;
using GoldenBread.Application.Common.Constants;
using GoldenBread.Application.Common.Exceptions;
using GoldenBread.Application.Contracts;
using GoldenBread.Application.Features.Auth.Dtos;
using GoldenBread.Domain.Enums;

namespace GoldenBread.Application.Features.Auth.Commands.Login;

public sealed class LoginCompanyCommandHandler(
    IUnitOfWork unitOfWork,
    IAccountRepository accountRepository,
    ICookieService cookieService,
    IPasswordHasher passwordHasher) : 
    IRequestHandler<LoginCommand, AuthResponse>
{
    public async Task<AuthResponse> Handle(
        LoginCommand command,
        CancellationToken ct)
    {
        var account = await accountRepository.GetByEmailAsync(command.Email, ct);

        if (account == null ||
            !HasAccess(account.AccountType, command.AccountType) ||
            !passwordHasher.Verify(command.Password, account.PasswordHash)) 
            throw new AuthException(ValidationErrorConstants.InvalidCredentials);

        account.SetSession();

        // Храним сессию в HttpContextAccessor только web-пользователей
        if (account.AccountType == AccountType.Company)
            await cookieService.SignInAsync(account.Session!);

        await unitOfWork.SaveChangesAsync(ct);

        return new AuthResponse(
            account.AccountId,
            account.VerificationStatus);
    }

    private bool HasAccess(AccountType accountType, PortalType portalType)
        => (accountType, portalType) switch
        { 
            (AccountType.Company, PortalType.Company) => true,
            (AccountType.User, PortalType.User) => true,
            _ => false,
        };
}