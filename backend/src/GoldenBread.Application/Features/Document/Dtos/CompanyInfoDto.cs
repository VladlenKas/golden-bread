namespace GoldenBread.Application.Features.Document.Dtos;

public record CompanyInfoDto(
    string Name,
    string Inn,
    string Ogrn,
    string? Address,
    string? Phone
);
