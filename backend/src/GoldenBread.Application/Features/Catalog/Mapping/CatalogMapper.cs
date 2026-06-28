using GoldenBread.Application.Features.Catalog.Dtos;
using GoldenBread.Domain.Entities;
using GoldenBread.Domain.Extensions;

namespace GoldenBread.Application.Features.Catalog.Mapping;

public class CatalogMapper
{
    public static ProductListItemResponse ToListItems(
        Product p,
        int? companyId,
        ProductSalesStatistics? stats = null)
    {
        var batch = SelectBatch(p.ProductBatches, companyId);

        var topSeason = stats?.SeasonalSales
            .Where(s => s.TotalUnitsSold > 0)
            .OrderByDescending(s => s.TotalUnitsSold)
            .FirstOrDefault();

        var badge = topSeason != null
            ? $"Топ {topSeason.Season.ToRussianString().ToLowerInvariant()} {topSeason.Year}"
            : null;

        return new ProductListItemResponse(
            ProductId: p.ProductId,
            Name: p.Name,
            Description: p.Description,
            ProductionTimeMinutes: p.ProductionTimeMinutes,
            CostPrice: p.CostPrice,

            CategoryId: p.CategoryId,
            CategoryName: p.Category.Name,
            CategoryColor: p.Category.Color,

            ProductBatchId: batch?.ProductBatchId ?? 0,
            QuantityPerBatch: batch?.QuantityUnits ?? 0,

            SalePrice: batch?.UnitPrice ?? 0,
            ImageUrl: p.ProductImages.FirstOrDefault()?.ImagePath,
            IsFavorite: p.Favorites.Any(f => f.CompanyId == companyId),
            QuantityInCart: p.GetQuantityInCart(companyId),

            TotalSoldAllTime: stats?.TotalSoldAllTime ?? 0,
            SeasonalSales: stats?.SeasonalSales ?? Array.Empty<SeasonalSalesData>(),
            TopSeasonBadge: badge,

            CreatedAt: p.CreatedAt
        );
    }

    public static ProductDetailResponse ToDetail(
        Product p,
        int? companyId,
        ProductSalesStatistics? stats = null)
    {
        var currentBatch = SelectBatch(p.ProductBatches, companyId);

        return new ProductDetailResponse(
            ProductId: p.ProductId,
            Name: p.Name,
            Description: p.Description,
            Weight: p.Weight,
            CostPrice: p.CostPrice,
            ProductionTimeMinutes: p.ProductionTimeMinutes,
            ShelfLifeDays: p.ShelfLifeDays,
            StorageTempMin: p.StorageTempMin,
            StorageTempMax: p.StorageTempMax,

            CategoryId: p.CategoryId,
            CategoryName: p.Category.Name,
            CategoryColor: p.Category.Color,

            CurrentBatchId: currentBatch?.ProductBatchId ?? 0,
            AvailableBatches: p.ProductBatches.Select(pb => new ProductBatchResponse(
                ProductBatchId: pb.ProductBatchId,
                QuantityPerBatch: pb.QuantityUnits,
                MarkupPercent: pb.MarkupPercent,
                UnitPrice: pb.UnitPrice,
                TotalPrice: pb.TotalPrice)).ToList(),

            ImageUrls: p.ProductImages.Select(i => i.ImagePath).ToList(),
            IsFavorite: p.Favorites.Any(f => f.CompanyId == companyId),
            QuantityInCart: p.GetQuantityInCart(companyId),
            TotalCostInCart: p.GetTotalCostInCart(companyId),

            Ingredients: p.Recipes.Select(r => new IngredientResponse(
                IngredientId: r.Ingredient.IngredientId,
                Name: r.Ingredient.Name,
                Quantity: r.Quantity,
                Unit: r.Ingredient.BaseUnit.ToString())).ToList(),

            TotalSoldAllTime: stats?.TotalSoldAllTime ?? 0,
            SeasonalSales: stats?.SeasonalSales ?? Array.Empty<SeasonalSalesData>()
        );
    }

    private static ProductBatch? SelectBatch(
        IEnumerable<ProductBatch> batches,
        int? companyId)
    {
        var batchList = batches.ToList();

        var cartBatch = batchList
            .FirstOrDefault(pb => pb.CartItems
                .Any(ci => ci.CompanyId == companyId));

        return cartBatch ?? batchList
            .OrderBy(pb => pb.QuantityUnits)
            .FirstOrDefault();
    }
}