using GoldenBread.Application.Abstractions.Data;
using GoldenBread.Application.Abstractions.Data.Repositories;
using GoldenBread.Domain.Entities;

namespace GoldenBread.Infrastructure.Data.Repositories;

public sealed class ProductRepository(IGoldenBreadContext context) : IProductRepository
{
    public async Task<List<Product>> GetAll(CancellationToken ct = default)
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

    public async Task<Product?> GetByIdAsync(int id, CancellationToken ct = default)
    {
        return await context.Products
            .Include(p => p.Category)
            .Include(p => p.ProductImages)
            .Include(p => p.ProductBatches)
            .Include(p => p.Recipes)
            .FirstOrDefaultAsync(p => p.ProductId == id, ct);
    }

    public async Task<Product?> GetByIdWithDetailsAsync(int id, CancellationToken ct = default)
    {
        return await context.Products
            .AsNoTracking()
            .AsSplitQuery()
            .Include(p => p.Category)
            .Include(p => p.ProductImages)
            .Include(p => p.ProductBatches)
                .ThenInclude(pb => pb.CartItems)
            .Include(p => p.Favorites)
            .Include(p => p.Recipes)
                .ThenInclude(r => r.Ingredient)
            .FirstOrDefaultAsync(p => p.ProductId == id, ct);
    }

    public async Task AddAsync(Product product, CancellationToken ct = default)
    {
        await context.Products.AddAsync(product, ct);
    }
}
