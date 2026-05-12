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

    public async Task AddAsync(SupplierIngredient entity, CancellationToken ct = default)
    {
        await context.SupplierIngredients.AddAsync(entity, ct);
    }
}