using Avalonia.Platform;
using System.Text.Json;

namespace GoldenBread.Desktop.Infrastructure.Helpers;

public static class JsonHelper
{
    private static readonly JsonSerializerOptions _jsonOptions = new()
    {
        PropertyNameCaseInsensitive = true
    };

    /// <summary>
    /// Десериализация из json в тип класса
    /// </summary>
    public static T Load<T>(string path) where T : class
    {
        var uri = new Uri(path);
        using var stream = AssetLoader.Open(uri);
        using var reader = new StreamReader(stream);
        return JsonSerializer.Deserialize<T>(reader.ReadToEnd(), _jsonOptions)!;
    }
}
