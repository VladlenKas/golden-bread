using Avalonia.Data.Converters;
using Avalonia.Media;
using System.Globalization;

namespace GoldenBread.Desktop.UI.Converters;

public class HexColorConverter : IValueConverter
{
    public object? Convert(object? value, Type targetType, object? parameter, CultureInfo culture)
    {
        var hex = value as string;
        if (string.IsNullOrWhiteSpace(hex))
            return new SolidColorBrush(Colors.Transparent);

        var withHash = hex.StartsWith('#') ? hex : '#' + hex;

        if (Color.TryParse(withHash, out var color))
            return new SolidColorBrush(color);

        return new SolidColorBrush(Colors.Transparent);
    }

    public object? ConvertBack(object? value, Type targetType, object? parameter, CultureInfo culture)
        => throw new NotSupportedException();
}
