namespace GoldenBread.Desktop.Features.References.Suppliers.Models;

public record class SupplierDto(
    int SupplierId,
    string Name,
    string? Email,
    string? Phone,
    string? Address);