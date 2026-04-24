using System.Text.Json.Serialization;

namespace GoldenBread.Desktop.Configuration.Models;

public sealed class AppPageConfig
{
    [JsonPropertyName("key")]
    public string Key { get; set; } = string.Empty;

    [JsonPropertyName("title")]
    public string Title { get; set; } = string.Empty;

    [JsonPropertyName("showInWindow")]
    public bool ShowInWindow { get; set; } = true;

    [JsonPropertyName("order")]
    public int Order { get; set; }
}
