using GoldenBread.Desktop.Features.Common.Account;

namespace GoldenBread.Desktop.UI.Helpers;

public static class LocalizedRoles
{
    public sealed record RoleFilterOption(UserRole? Value, string Display);

    /// <summary>
    /// Для форм
    /// </summary>
    public static Dictionary<UserRole, string> Roles { get; } = new()
    {
        [UserRole.Technologist] = "Технолог",
        [UserRole.CommercialManager] = "Коммерческий менеджер",
    };

    /// <summary>
    ///  Для фильтров
    /// </summary>
    public static List<RoleFilterOption> RolesFilters { get; } = new()
    {
        new(null, "Все должности"),
        new(UserRole.Technologist, "Технолог"),
        new(UserRole.CommercialManager, "Коммерческий менеджер"),
    };

    /// <summary>
    /// Для таблиц и списков
    /// </summary>
    public static string RolesTable(UserRole role) => role switch
    {
        UserRole.Technologist => "Технолог",
        UserRole.CommercialManager => "Коммерческий менеджер",
        _ => "-"
    };
}