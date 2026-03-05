using GoldenBread.Domain.Enums;

namespace GoldenBread.Application.Features.CompanyProfile.Dtos;

public sealed class ProfileResponse
{
    public string Email { get; set; } = null!;
    public string Name { get; set; } = null!;
    public string Inn { get; set; } = null!;
    public string Ogrn { get; set; } = null!;
    public string? Phone { get; set; }
    public string? Address { get; set; }
}
