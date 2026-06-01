using SukiUI.Models;

namespace GoldenBread.Desktop.UI.Helpers;

public sealed class LocalizedTheme(SukiColorTheme theme)
{
    public SukiColorTheme Theme => theme;

    public string DisplayName => Theme.DisplayName switch
    {
        "Blue" => "Классическая",
        "Orange" => "Брендовая",
        _ => Theme.DisplayName
    };

    public static bool IsAllowed(SukiColorTheme theme) => theme.DisplayName switch
    {
        "Blue" => true,
        "Orange" => true,
        _ => false
    };
}