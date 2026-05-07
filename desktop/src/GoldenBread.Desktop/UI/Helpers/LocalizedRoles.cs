using GoldenBread.Desktop.Features.Common.Models;

namespace GoldenBread.Desktop.UI.Helpers;

public static class LocalizedRoles
{
    public sealed record RoleFilterOption(UserRole? Value, string Display);

    /// <summary>
    /// Используется для форм
    /// </summary>
    public static Dictionary<UserRole, string> Roles { get; } = new()
    {
        [UserRole.Technologist] = "Технолог",
        [UserRole.CommercialManager] = "Коммерческий менеджер",
    };

    /// <summary>
    ///  Используется для фильтров
    /// </summary>
    public static List<RoleFilterOption> RolesFilters { get; } = new()
    {
        new(null, "Все должности"),
        new(UserRole.Technologist, "Технолог"),
        new(UserRole.CommercialManager, "Коммерческий менеджер"),
    };
}