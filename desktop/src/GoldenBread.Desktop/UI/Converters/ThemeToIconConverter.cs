using Avalonia.Data.Converters;
using Avalonia.Styling;
using System.Globalization;

namespace GoldenBread.Desktop.UI.Converters;

public class ThemeToIconConverter : IValueConverter
{
    public object Convert(object? value, Type targetType, object? parameter, CultureInfo culture)
    {
        return (ThemeVariant)value! == ThemeVariant.Light ? "WeatherNight" : "WeatherSunny";
    }
    public object ConvertBack(object? value, Type targetType, object? parameter, CultureInfo culture)
        => throw new NotSupportedException();
}