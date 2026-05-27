using GoldenBread.Domain.Enums;

namespace GoldenBread.Application.Features.Auth.Dtos;

public sealed record AuthResponse(
    int Id,
    string? Session,
    UserRole? Role,
    VerificationStatus VerificationStatus,
    string? UserInfo = null,
    string? SessionInfo = null)
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
                account.VerificationStatus,
                $" {RolesLocalized(account.User.Role)}: {account.User.Lastname} {account.User.Firstname}",
                $"Сессия активна до: {account.SessionExpiresAt!.Value.LocalDateTime:dd.MM.yyyy HH:mm}");
        }
    }

    private static string RolesLocalized(UserRole role) => role switch
    {
        UserRole.Technologist => "Технолог",
        UserRole.CommercialManager => "Коммерческий менеджер",
        _ => "-"
    };
}