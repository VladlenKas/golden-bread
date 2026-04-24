namespace GoldenBread.Application.Features.CompanyProfile.Dtos;

public sealed record ProfileResponse(
    string Email,
    string Name,
    string Inn,
    string Ogrn,
    string? Phone,
    string? Address);
