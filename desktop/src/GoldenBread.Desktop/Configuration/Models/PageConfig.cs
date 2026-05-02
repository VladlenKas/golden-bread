using System.Text.Json.Serialization;

namespace GoldenBread.Desktop.Configuration.Models;

public sealed class PageConfig
{
    [JsonPropertyName("key")]
    public string Key { get; set; } = string.Empty;

    [JsonPropertyName("title")]
    public string Title { get; set; } = string.Empty;

    [JsonPropertyName("order")]
    public int Order { get; set; }
}