using Avalonia.Data.Converters;
using Avalonia.Media;
using System.Globalization;

namespace GoldenBread.Desktop.UI.Converters;

public class HexColorConverter : IValueConverter
{
    public object? Convert(object? value, Type targetType, object? parameter, CultureInfo culture)
    {
        if (value is string hex)
        {
            if (!hex.StartsWith('#'))
                hex = "#" + hex;

            if (Color.TryParse(hex, out var color))
                return new SolidColorBrush(color);
        }
        return new SolidColorBrush(Colors.Gray);
    }

    public object? ConvertBack(object? value, Type targetType, object? parameter, CultureInfo culture)
        => throw new NotSupportedException();
}
