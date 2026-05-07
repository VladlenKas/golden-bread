using GoldenBread.Desktop.Features.Common.Models;

namespace GoldenBread.Desktop.UI.Helpers;

public static class LocalizedVerificationStatuses
{
    public sealed record StatusesFilterOption(VerificationStatus? Value, string Display);

    /// <summary>
    /// Для форм
    /// </summary>
    public static Dictionary<VerificationStatus, string> Statuses { get; } = new()
    {
        [VerificationStatus.Pending] = "На рассмотрении",
        [VerificationStatus.Approved] = "Подтверждён",
        [VerificationStatus.Rejected] = "Отклонён",
        [VerificationStatus.Suspended] = "Приостановлен"
    };

    /// <summary>
    ///  Для фильтров
    /// </summary>
    public static List<StatusesFilterOption> StatusesFilters { get; } = new()
    {
        new(null, "Все статусы"),
        new(VerificationStatus.Pending, "На рассмотрении"),
        new(VerificationStatus.Approved, "Подтверждён"),
        new(VerificationStatus.Rejected, "Отклонён"),
        new(VerificationStatus.Suspended, "Приостановлен"),
    };

    /// <summary>
    /// Для таблиц и списков
    /// </summary>
    public static string StatusesTable(VerificationStatus status) => status switch
    {
        VerificationStatus.Pending => "На рассмотрении",
        VerificationStatus.Approved => "Подтверждён",
        VerificationStatus.Rejected => "Отклонён",
        VerificationStatus.Suspended => "Приостановлен",
        _ => "-"
    };
}