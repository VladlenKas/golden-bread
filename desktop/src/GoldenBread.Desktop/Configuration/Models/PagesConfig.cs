using System.Text.Json.Serialization;

namespace GoldenBread.Desktop.Configuration.Models;

public sealed class PagesConfig
{
    [JsonPropertyName("pages")]
    public List<PageConfig> Pages { get; set; } = new();
}