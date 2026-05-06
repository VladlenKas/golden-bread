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
        return await context.Suppliers.AnyAsync(a =>
            a.Name == name &&
            (!excludeId.HasValue || a.SupplierId != excludeId.Value), ct);
    }

    public async Task<IReadOnlyList<Supplier>> GetAllAsync(CancellationToken ct = default)
    {
        return await context.Suppliers
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