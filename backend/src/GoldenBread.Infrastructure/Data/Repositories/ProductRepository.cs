using GoldenBread.Application.Abstractions.Data;
using GoldenBread.Application.Abstractions.Data.Repositories;
using GoldenBread.Domain.Entities;

namespace GoldenBread.Infrastructure.Data.Repositories;

public sealed class ProductRepository(IGoldenBreadContext context) : IProductRepository
{
    public async Task<List<Product>> GetAll(CancellationToken ct)
    {
        return await context.Products
            .AsNoTracking()
            .AsSplitQuery()
            .Include(p => p.Category)
            .Include(p => p.ProductImages)
            .Include(p => p.ProductBatches)
                .ThenInclude(pb => pb.CartItems)
            .Include(p => p.Favorites)
            .ToListAsync(ct);
    }
}
