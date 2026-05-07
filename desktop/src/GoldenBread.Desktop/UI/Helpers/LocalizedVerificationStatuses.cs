using GoldenBread.Desktop.Features.Common.Models;
using static GoldenBread.Desktop.UI.Helpers.LocalizedRoles;

namespace GoldenBread.Desktop.UI.Helpers;

public static class LocalizedVerificationStatuses
{
    public sealed record StatusesFilterOption(VerificationStatus? Value, string Display);

    /// <summary>
    /// Используется для форм
    /// </summary>
    public static Dictionary<VerificationStatus, string> Statuses { get; } = new()
    {
        [VerificationStatus.Pending] = "На рассмотрении",
        [VerificationStatus.Approved] = "Подтверждён",
        [VerificationStatus.Rejected] = "Отклонён",
        [VerificationStatus.Suspended] = "Приостановлен"
    };

    /// <summary>
    ///  Используется для фильтров
    /// </summary>
    public static List<StatusesFilterOption> StatusesFilters { get; } = new()
    {
        new(null, "Все статусы"),
        new(VerificationStatus.Pending, "На рассмотрении"),
        new(VerificationStatus.Approved, "Подтверждён"),
        new(VerificationStatus.Rejected, "Отклонён"),
        new(VerificationStatus.Suspended, "Приостановлен"),
    };
}