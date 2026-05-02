using System.ComponentModel.DataAnnotations.Schema;

namespace GoldenBread.Desktop.Features.Common.Models;

public enum UserRole
{
    [NotMapped] None,
    Technologist,
    CommercialManager,
}