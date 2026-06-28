using GoldenBread.Application.Abstractions.Data;
using GoldenBread.Application.Abstractions.Data.Services;
using GoldenBread.Application.Features.Catalog.Dtos;
using GoldenBread.Domain.Entities;
using GoldenBread.Domain.Enums;
using GoldenBread.Domain.Extensions;
using Microsoft.EntityFrameworkCore;

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

        var salesStats = await GetSalesStatisticsAsync(ct);

        return new CatalogData(products, categories, salesStats);
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

    private async Task<Dictionary<int, ProductSalesStatistics>> GetSalesStatisticsAsync(CancellationToken ct)
    {
        var items = await context.OrderItems
            .AsNoTracking()
            .Where(oi => oi.Order.Status != OrderStatus.Canceled
                      && oi.BatchId != null)
            .Select(oi => new
            {
                ProductId = oi.Batch.ProductId,
                TotalUnits = oi.TotalUnits,
                EndDate = oi.Order.EndDate!
            })
            .ToListAsync(ct);

        var result = new Dictionary<int, ProductSalesStatistics>();

        foreach (var g in items.GroupBy(x => x.ProductId))
        {
            var totalAllTime = g.Sum(x => x.TotalUnits);

            var seasonal = g
                .GroupBy(x => new { x.EndDate.Year, Season = x.EndDate.GetSeason() })
                .Select(sg => new SeasonalSalesData(
                    sg.Key.Season,
                    sg.Key.Year,
                    sg.Sum(x => x.TotalUnits)))
                .ToList();

            result[g.Key] = new ProductSalesStatistics(totalAllTime, seasonal);
        }

        return result;
    }
}