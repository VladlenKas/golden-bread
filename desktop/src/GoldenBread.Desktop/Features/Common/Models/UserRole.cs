using System.ComponentModel.DataAnnotations.Schema;
using System.Text.Json.Serialization;

namespace GoldenBread.Desktop.Features.Common.Models;

public enum UserRole
{
    Technologist,
    CommercialManager,
    [NotMapped] 
    [JsonIgnore]
    None,
}