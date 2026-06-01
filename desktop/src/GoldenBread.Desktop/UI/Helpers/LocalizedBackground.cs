using SukiUI.Enums;

namespace GoldenBread.Desktop.UI.Helpers;

public sealed class LocalizedBackground(SukiBackgroundStyle style)
{
    public SukiBackgroundStyle Style => style;

    public string DisplayName => Style switch
    {
        SukiBackgroundStyle.Gradient => "Градиент",
        SukiBackgroundStyle.Flat => "Плоский",
        _ => Style.ToString()
    };

    public static bool IsAllowed(SukiBackgroundStyle style) => style switch
    {
        SukiBackgroundStyle.Gradient => true,
        SukiBackgroundStyle.Flat => true,
        _ => false
    };
}