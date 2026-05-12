using GoldenBread.Domain.Enums;

namespace GoldenBread.Application.Features.Companies.Dtos;

public record CompaniesListResponse(List<CompanyListItem> CompaniesList);

public record CompanyListItem(
    int AccountId,
    int CompanyId,
    string Name,
    string Inn,
    string Ogrn,
    string? Phone,
    string? Address,
    string Email,
    VerificationStatus VerificationStatus,
    DateTimeOffset CreatedAt,
    DateTimeOffset? SessionExpiresAt,
    bool CanDelete);
