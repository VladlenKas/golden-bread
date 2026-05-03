namespace GoldenBread.Application.Features.Suppliers.Dtos;

public record SupplierListItem(
    int SupplierId,
    string Name,
    string? Email,
    string? Phone,
    string? Address);

public record SuppliersListResponse(List<SupplierListItem> SuppliersList);