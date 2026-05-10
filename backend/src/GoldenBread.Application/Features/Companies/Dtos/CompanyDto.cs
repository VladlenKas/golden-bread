using GoldenBread.Domain.Entities;

namespace GoldenBread.Application.Features.Companies.Dtos;

public record class CompanyDto(
    int CompanyId,
    string Name,
    string Inn,
    string Ogrn,
    string? Phone,
    string? Address);