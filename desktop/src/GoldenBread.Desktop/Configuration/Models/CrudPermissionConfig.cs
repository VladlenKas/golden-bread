using System.Text.Json.Serialization;

namespace GoldenBread.Desktop.Configuration.Models;

public sealed class CrudPermissionConfig
{
    [JsonPropertyName("view")]
    public bool View { get; set; }

    [JsonPropertyName("create")]
    public bool Create { get; set; }

    [JsonPropertyName("update")]
    public bool Update { get; set; }

    [JsonPropertyName("delete")]
    public bool Delete { get; set; }
}
