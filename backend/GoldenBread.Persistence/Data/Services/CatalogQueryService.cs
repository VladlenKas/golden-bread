using GoldenBread.Application.Abstractions.Data;
using GoldenBread.Application.Abstractions.Data.Services;
using GoldenBread.Domain.Entities;

namespace GoldenBread.Infrastructure.Data.Services;

public sealed class CatalogQueryService(IGoldenBreadContext context) : ICatalogQueryService
{
    public async Task<CatalogData> GetCatalogAsync(int? companyId, CancellationToken ct)
    {
        var products = await context.Products
            .AsNoTracking()
            .AsSplitQuery()
            .Include(p => p.Category)
            .Include(p => p.ProductImages)
            .Include(p => p.ProductBatches)
                .ThenInclude(pb => pb.CartItems)
            .Include(p => p.Favorites)
            .ToListAsync(ct);

        var categories = await context.ProductCategories
            .AsNoTracking()
            .Select(c => new CategoryWithCount(
                c.ProductCategoryId,
                c.Name,
                c.Color,
                c.Products.Count))
            .ToListAsync(ct);

        return new CatalogData(products, categories);
    }

    public async Task<Product?> GetProductDetailAsync(int productId, CancellationToken ct)
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
            .FirstOrDefaultAsync(p => p.ProductId == productId, ct);
    }
}

