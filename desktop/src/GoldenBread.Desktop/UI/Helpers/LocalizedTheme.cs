using SukiUI.Models;
namespace GoldenBread.Desktop.UI.Helpers;

public class LocalizedTheme
{
    public SukiColorTheme Theme { get; }
    public string DisplayName => Theme.DisplayName switch
    {
        "Red" => "Красная",
        "Green" => "Зелёная",
        "Blue" => "Синяя",
        "Orange" => "Оранжевая",
        "Purple" => "Фиолетовая",
        "Pink" => "Розовая",
        _ => Theme.DisplayName
    };

    public LocalizedTheme(SukiColorTheme theme) => Theme = theme;
}