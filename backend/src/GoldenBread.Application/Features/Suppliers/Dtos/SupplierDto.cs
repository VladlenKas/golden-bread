namespace GoldenBread.Application.Features.Suppliers.Dtos;

public record class SupplierDto(
    int SupplierId,
    string Name,
    string? Email,
    string? Phone,
    string? Address);