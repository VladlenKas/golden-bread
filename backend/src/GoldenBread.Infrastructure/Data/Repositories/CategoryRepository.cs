using GoldenBread.Application.Abstractions.Data;
using GoldenBread.Application.Abstractions.Data.Repositories;
using GoldenBread.Domain.Entities;

namespace GoldenBread.Infrastructure.Data.Repositories;

public sealed class CategoryRepository(IGoldenBreadContext context) : ICategoryRepository
{
    public async Task<List<ProductCategory>> GetAllAsync(CancellationToken ct = default)
    {
        return await context.ProductCategories
            .Where(c => c.DeletedAt == null)
            .Include(c => c.Products)
            .AsNoTracking()
            .ToListAsync(ct);
    }

    public async Task<ProductCategory?> GetByIdAsync(int id, CancellationToken ct = default)
    {
        return await context.ProductCategories
            .FirstOrDefaultAsync(c => c.ProductCategoryId == id, ct);
    }

    public async Task AddAsync(ProductCategory category, CancellationToken ct = default)
    {
        await context.ProductCategories.AddAsync(category, ct);
    }

    public async Task<bool> ExistsByNameAsync(
        string name,
        int? excludeId = null,
        CancellationToken ct = default)
    {
        if (string.IsNullOrWhiteSpace(name))
            return false;

        name = name.Trim().ToLower();

        return await context.ProductCategories.AnyAsync(c =>
            c.Name != null &&
            c.Name.Trim().ToLower() == name &&
            (!excludeId.HasValue || c.ProductCategoryId != excludeId.Value), ct);
    }
}
