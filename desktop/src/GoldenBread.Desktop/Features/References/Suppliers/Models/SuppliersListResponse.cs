using GoldenBread.Desktop.Infrastructure.Helpers;

namespace GoldenBread.Desktop.Features.References.Suppliers.Models;

public record SuppliersListResponse(List<SupplierListItem> SuppliersList);

public record SupplierListItem(
    int SupplierId,
    string Name,
    string? Email,
    string? Phone,
    string? Address)
{
    public string SearchText => $"{Name}{Email}{Phone}{Address}";
    public string? PhoneFormatted => InputFormatter.FormatPhone(Phone);
};