namespace GoldenBread.Desktop.Features.Administration.Companies.Models;

public record class CompanyDto(
    int CompanyId,
    string Name,
    string Inn,
    string Ogrn,
    string? Phone,
    string? Address);
