using Avalonia.Data.Converters;
using Avalonia.Media.Imaging;
using GoldenBread.Desktop.Configuration.Files;
using System.Globalization;

namespace GoldenBread.Desktop.UI.Converters;

public class ImagePathToBitmapConverter : IValueConverter
{
    public static ImagePathToBitmapConverter Instance { get; } = new();

    public object? Convert(object? value, Type targetType, object? parameter, CultureInfo culture)
    {
        if (value is not string path || string.IsNullOrEmpty(path))
            return null;

        try
        {
            var fullUrl = $"{AppSettings.ApiDbUploadUrl}/{path}";

            // Для URL нужно загружать в поток
            using var client = new HttpClient();
            var stream = client.GetStreamAsync(fullUrl).Result; // .Result — костыль, лучше async loading
            return new Bitmap(stream);
        }
        catch
        {
            return null;
        }
    }

    public object ConvertBack(object? value, Type targetType, object? parameter, CultureInfo culture)
        => throw new NotSupportedException();
}