using SukiUI.Models;

namespace GoldenBread.Desktop.UI.Helpers;

public class LocalizedTheme(SukiColorTheme theme)
{
    public SukiColorTheme Theme => theme;
    public string DisplayName => Theme.DisplayName switch
    {
        "Red" => "Красная",
        "Green" => "Зелёная",
        "Blue" => "Синяя",
        "Orange" => "Оранжевая",
        "Purple" => "Фиолетовая",
        "Pink" => "Розовая",
        "White" => "Белая",
        "Black" => "Темная",
        _ => Theme.DisplayName
    };
}   