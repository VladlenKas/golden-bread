namespace GoldenBread.Application.Features.Suppliers.Dtos;

public record SupplierListItem(
    int SupplierId,
    string Name,
    string? Email,
    string? Phone,
    string? Address,
    bool CanDelete);

public record SuppliersListResponse(List<SupplierListItem> SuppliersList);