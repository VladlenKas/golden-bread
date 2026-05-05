using GoldenBread.Domain.Entities;
using GoldenBread.Domain.Enums;

namespace GoldenBread.Application.Features.Auth.Dtos;

public sealed record AuthResponse(
    int Id,
    string? Session,
    UserRole? Role,
    VerificationStatus VerificationStatus)
{
    public static AuthResponse Response(DbEntities.Account account)
    {
        if (account.AccountType == AccountType.Company)
        {
            return new AuthResponse(
                account.AccountId,
                null,
                null,
                account.VerificationStatus);
        }
        else
        {
            return new AuthResponse(
                account.AccountId,
                account.Session,
                account.User!.Role,
                account.VerificationStatus);
        }
    }
}