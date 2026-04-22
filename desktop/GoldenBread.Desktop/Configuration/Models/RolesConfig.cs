using System.Collections.Generic;
using System.Text.Json.Serialization;

namespace GoldenBread.Desktop.Configuration.Models;

public sealed class RolesConfig
{
    [JsonPropertyName("roles")]
    public List<RoleConfig> Roles { get; set; } = new();
}