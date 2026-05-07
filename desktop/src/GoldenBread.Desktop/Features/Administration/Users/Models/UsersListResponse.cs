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
    VerificationStatus VerificationStatus)
{
    public string SearchText = $"{UserId}{Fullname}{Birthday}{Role}{Email}{VerificationStatus}";
    public string LocalizedRole => LocalizedRoles.RolesTable(Role);
    public string LocalizedStatus => LocalizedVerificationStatuses.StatusesTable(VerificationStatus);
};