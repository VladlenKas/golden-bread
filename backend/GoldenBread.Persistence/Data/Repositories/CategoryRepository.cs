using GoldenBread.Application.Abstractions.Data;
using GoldenBread.Application.Abstractions.Data.Repositories;
using GoldenBread.Domain.Entities;

namespace GoldenBread.Infrastructure.Data.Repositories;

public sealed class CategoryRepository(IGoldenBreadContext context) : ICategoryRepository
{
    public async Task<List<ProductCategory>> GetAll(CancellationToken ct)
    {
        return await context.ProductCategories
            .AsNoTracking()
            .Include(c => c.Products)
            .ToListAsync(ct);
    }
}
