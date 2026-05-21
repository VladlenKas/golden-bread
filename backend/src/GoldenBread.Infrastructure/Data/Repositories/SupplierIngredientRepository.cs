using GoldenBread.Application.Abstractions.Data;
using GoldenBread.Application.Abstractions.Data.Repositories;
using GoldenBread.Domain.Entities;

namespace GoldenBread.Infrastructure.Data.Repositories;

public class SupplierIngredientRepository(IGoldenBreadContext context) : ISupplierIngredientRepository
{
    public async Task<IReadOnlyList<SupplierIngredient>> GetAllAsync(CancellationToken ct = default)
    {
        return await context.SupplierIngredients
            .Where(si => si.DeletedAt == null)
            .IgnoreQueryFilters()
            .Include(si => si.Supplier)
            .Include(si => si.Ingredient)
            .Include(si => si.IngredientBatches)
            .AsNoTracking()
            .ToListAsync(ct);
    }

    public async Task<SupplierIngredient?> GetByIdAsync(int id, CancellationToken ct = default)
    {
        return await context.SupplierIngredients
            .Where(si => si.DeletedAt == null)
            .IgnoreQueryFilters()
            .Include(si => si.Supplier)
            .Include(si => si.Ingredient)
            .Include(si => si.IngredientBatches)
            .FirstOrDefaultAsync(si => si.SupplierIngredientId == id, ct);
    }

    public async Task<bool> ExistsByNameAsync(
        int supplierId,
        string name,
        int? excludeId = null,
    CancellationToken ct = default)
    {
        if (string.IsNullOrWhiteSpace(name))
            return false;

        name = name.Trim().ToLower();

        return await context.SupplierIngredients
            .Where(x => 
                x.DeletedAt == null &&
                x.SupplierId == supplierId)
            .AnyAsync(x =>
                x.Name != null &&
                x.Name.Trim().ToLower() == name &&
                (!excludeId.HasValue || x.IngredientId != excludeId.Value),
                ct);
    }

    public async Task AddAsync(SupplierIngredient entity, CancellationToken ct = default)
    {
        await context.SupplierIngredients.AddAsync(entity, ct);
    }
}