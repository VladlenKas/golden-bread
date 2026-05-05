using GoldenBread.Desktop.Features.Common.Models;

namespace GoldenBread.Desktop.UI.Helpers;

public static class LocalizedRoles
{
    public static Dictionary<UserRole, string> Roles { get; } = new()
    {
        [UserRole.Technologist] = "Технолог",
        [UserRole.CommercialManager] = "Коммерческий менеджер"
    };
}