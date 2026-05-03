using SukiUI.Enums;

namespace GoldenBread.Desktop.UI.Helpers;

public class LocalizedBackground
{
    public SukiBackgroundStyle Style { get; }
    public string DisplayName => Style switch
    {
        SukiBackgroundStyle.GradientDarker => "Темный градиент",
        SukiBackgroundStyle.GradientSoft => "Мягкий градиент",
        SukiBackgroundStyle.Gradient => "Градиент",
        SukiBackgroundStyle.Flat => "Плоский",
        SukiBackgroundStyle.Bubble => "Пузырьки",
        _ => Style.ToString()
    };

    public LocalizedBackground(SukiBackgroundStyle style) => Style = style;
}