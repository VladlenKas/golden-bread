using GoldenBread.Application.Abstractions.Data;
using GoldenBread.Application.Abstractions.Data.Repositories;
using GoldenBread.Domain.Entities;

namespace GoldenBread.Infrastructure.Data.Repositories;

public class SupplierRepository(IGoldenBreadContext context) : ISupplierRepository
{
    public async Task<bool> ExistsByNameAsync(
        string name,
        int? excludeId = null,
        CancellationToken ct = default)
    {
        name = name.Trim().ToLower();

        return await context.Suppliers.AnyAsync(a =>
            a.Name != null &&
            a.Name.Trim().ToLower() == name &&
            (!excludeId.HasValue || a.SupplierId != excludeId.Value), ct);
    }
    public async Task<bool> ExistsByEmailAsync(
        string? email,
        int? excludeId = null,
        CancellationToken ct = default)
    {
        if (string.IsNullOrWhiteSpace(email)) 
            return false;

        return await context.Suppliers.AnyAsync(a =>
            a.Email == email &&
            (!excludeId.HasValue || a.SupplierId != excludeId.Value), ct);
    }
    public async Task<bool> ExistsByPhoneAsync(
        string? phone,
        int? excludeId = null,
        CancellationToken ct = default)
    {
        if (string.IsNullOrWhiteSpace(phone)) 
            return false;

        return await context.Suppliers.AnyAsync(a =>
            a.Phone == phone &&
            (!excludeId.HasValue || a.SupplierId != excludeId.Value), ct);
    }
    public async Task<bool> ExistsByAddressAsync(
        string? address,
        int? excludeId = null,
        CancellationToken ct = default)
    {
        if (string.IsNullOrWhiteSpace(address))
            return false;

        address = address.Trim().ToLower();

        return await context.Suppliers.AnyAsync(a =>
            a.Address != null && 
            a.Address.Trim().ToLower() == address &&
            (!excludeId.HasValue || a.SupplierId != excludeId.Value), ct);
    }

    public async Task<IReadOnlyList<Supplier>> GetAllAsync(CancellationToken ct = default)
    {
        return await context.Suppliers
            .Include(s => s.SupplierIngredients)
            .AsNoTracking()
            .ToListAsync(ct);
    }

    public async Task<Supplier?> GetByIdAsync(int id, CancellationToken ct = default)
    {
        return await context.Suppliers
            .FirstOrDefaultAsync(s => s.SupplierId == id, ct);
    }

    public async Task AddAsync(Supplier supplier, CancellationToken ct = default)
    {
        await context.Suppliers.AddAsync(supplier, ct);
    }
}