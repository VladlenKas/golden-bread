using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace GoldenBread.Desktop.Configuration.Models;

public sealed class RoleConfig
{
    [JsonPropertyName("key")]
    public string Key { get; set; } = string.Empty;

    [JsonPropertyName("title")]
    public string Title { get; set; } = string.Empty;

    [JsonPropertyName("permissions")]
    public Dictionary<string, CrudPermissionConfig> Permissions { get; set; } = new();
}