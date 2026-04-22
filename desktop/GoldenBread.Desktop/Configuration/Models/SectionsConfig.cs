using System.Collections.Generic;
using System.Text.Json.Serialization;

namespace GoldenBread.Desktop.Configuration.Models;

public sealed class SectionsConfig
{
    [JsonPropertyName("sections")]
    public List<AppSectionConfig> Sections { get; set; } = new();
}