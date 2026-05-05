using GoldenBread.Desktop.Features.Common.Models;

namespace GoldenBread.Desktop.UI.Helpers;

public static class LocalizedVerificationStatuses
{
    public static Dictionary<VerificationStatus, string> Statuses { get; } = new()
    {
        [VerificationStatus.Pending] = "На рассмотрении",
        [VerificationStatus.Approved] = "Подтверждён",
        [VerificationStatus.Rejected] = "Отклонён",
        [VerificationStatus.Suspended] = "Приостановлен"
    };
}