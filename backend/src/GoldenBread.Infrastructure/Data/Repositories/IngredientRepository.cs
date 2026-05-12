using GoldenBread.Application.Abstractions.Data;
using GoldenBread.Application.Abstractions.Data.Repositories;
using GoldenBread.Domain.Entities;


namespace GoldenBread.Infrastructure.Data.Repositories;

public class IngredientRepository(IGoldenBreadContext context) : IIngredientRepository
{
    public async Task<IReadOnlyList<Ingredient>> GetAllAsync(CancellationToken ct = default)
    {
        return await context.Ingredients
            .Where(i => i.DeletedAt == null)
            .AsNoTracking()
            .ToListAsync(ct);
    }

    public async Task<Ingredient?> GetByIdAsync(int id, CancellationToken ct = default)
    {
        return await context.Ingredients
            .FirstOrDefaultAsync(i => i.IngredientId == id, ct);
    }

    public async Task<Ingredient?> GetByIdWithRelationsAsync(int id, CancellationToken ct = default)
    {
        return await context.Ingredients
            .Include(i => i.Recipes)
            .Include(i => i.SupplierIngredients)
            .FirstOrDefaultAsync(i => i.IngredientId == id, ct);
    }

    public async Task AddAsync(Ingredient ingredient, CancellationToken ct = default)
    {
        await context.Ingredients.AddAsync(ingredient, ct);
    }
}