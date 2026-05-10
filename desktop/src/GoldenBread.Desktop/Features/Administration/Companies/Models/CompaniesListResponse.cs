using GoldenBread.Desktop.Features.Common.Models;
using GoldenBread.Desktop.Infrastructure.Helpers;
using GoldenBread.Desktop.UI.Helpers;

namespace GoldenBread.Desktop.Features.Administration.Companies.Models;

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
    DateTimeOffset? SessionExpiresAt)
{
    public string SearchText = $"{CompanyId}{Name}{Inn}{Ogrn}{Phone}{Address}{Email}{VerificationStatus}";
    public string LocalizedStatus => LocalizedVerificationStatuses.StatusesTable(VerificationStatus);
    public string PhoneFormatted => InputFormatter.FormatPhone(Phone) ?? "-";
    public string AddressFormatted => Address ?? "-";
    public string OgrnFormatted => InputFormatter.FormatOgrn(Ogrn) ?? "-";
    public string InnFormatted => InputFormatter.FormatInn(Inn) ?? "-";
    public string CreatedAtFormatted =>
        CreatedAt.ToLocalTime().ToString("dd.MM.yyyy HH:mm");
    public string SessionExpiresAtFormatted => 
        SessionExpiresAt?.ToLocalTime().ToString("dd.MM.yyyy HH:mm") ?? "Нет активной";
};