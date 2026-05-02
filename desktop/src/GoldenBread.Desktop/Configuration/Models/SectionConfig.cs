using System.Text.Json.Serialization;

namespace GoldenBread.Desktop.Configuration.Models;

public sealed class SectionConfig
{
    [JsonPropertyName("key")]
    public string Key { get; set; } = string.Empty;

    [JsonPropertyName("title")]
    public string Title { get; set; } = string.Empty;

    [JsonPropertyName("icon")]
    public string Icon { get; set; } = string.Empty;

    [JsonPropertyName("order")]
    public int Order { get; set; }

    [JsonPropertyName("pages")]
    public List<PageConfig> Pages { get; set; } = new();
}