using GoldenBread.Desktop.Features.Common.Models;

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
    public string LocalizedRole => Role switch
    {
        UserRole.Technologist => "Технолог",
        UserRole.CommercialManager => "Коммерческий менеджер",
        _ => "-"
    };

    public string LocalizedStatus => VerificationStatus switch
    {
        VerificationStatus.Pending => "На рассмотрении",
        VerificationStatus.Approved => "Подтверждён",
        VerificationStatus.Rejected => "Отклонён",
        VerificationStatus.Suspended => "Приостановлен",
        _ => "-"
    };
};