using System.Text.Json.Serialization;

namespace GoldenBread.Desktop.Configuration.Models;

public sealed class CrudPermissionConfig
{
    public bool View => true;

    [JsonPropertyName("create")]
    public bool Create { get; set; }

    [JsonPropertyName("update")]
    public bool Update { get; set; }

    [JsonPropertyName("delete")]
    public bool Delete { get; set; }
}
