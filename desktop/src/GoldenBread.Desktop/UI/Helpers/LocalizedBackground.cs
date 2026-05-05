using SukiUI.Enums;

namespace GoldenBread.Desktop.UI.Helpers;

public class LocalizedBackground(SukiBackgroundStyle style)
{
    public SukiBackgroundStyle Style { get; } = style;
    public string DisplayName => Style switch
    {
        SukiBackgroundStyle.GradientDarker => "Темный градиент",
        SukiBackgroundStyle.GradientSoft => "Мягкий градиент",
        SukiBackgroundStyle.Gradient => "Градиент",
        SukiBackgroundStyle.Flat => "Плоский",
        SukiBackgroundStyle.Bubble => "Пузырьки",
        _ => Style.ToString()
    };
}