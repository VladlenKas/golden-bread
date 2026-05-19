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
}
