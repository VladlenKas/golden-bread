using GoldenBread.Desktop.Features.Common.Models;
using GoldenBread.Desktop.UI.Helpers;

namespace GoldenBread.Desktop.Features.Administration.Users.Models;

public record UsersListResponse(List<UserListItem> UsersList);

public record UserListItem(
    int AccountId,
    int UserId,
    string Fullname,
    DateOnly Birthday,
    UserRole Role,
    string Email,
    VerificationStatus VerificationStatus,
    DateTimeOffset CreatedAt,
    DateTimeOffset? SessionExpiresAt)
{
    public string SearchText = $"{UserId}{Fullname}{Birthday}{Role}{Email}{VerificationStatus}";
    public string LocalizedRole => LocalizedRoles.RolesTable(Role);
    public string LocalizedStatus => LocalizedVerificationStatuses.StatusesTable(VerificationStatus);
    public string CreatedAtFormatted =>
        CreatedAt.ToLocalTime().ToString("dd.MM.yyyy HH:mm");
    public string SessionExpiresAtFormatted =>
        SessionExpiresAt?.ToLocalTime().ToString("dd.MM.yyyy HH:mm") ?? "Нет активной";
};