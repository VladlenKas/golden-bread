using GoldenBread.Application.Features.Catalog.Dtos;
using GoldenBread.Domain.Entities;
using GoldenBread.Domain.Extensions;

namespace GoldenBread.Application.Features.Catalog.Mapping;

public class CatalogMapper
{
    public static ProductListItemResponse ToListItems(DbEntities.Product p, int? companyId)
    {
        var batch = SelectBatch(p.ProductBatches, companyId);

        return new ProductListItemResponse(
            ProductId: p.ProductId,
            Name: p.Name,
            Description: p.Description,
            ProductionTimeMinutes: p.ProductionTimeMinutes,

            CategoryId: p.CategoryId,
            CategoryName: p.Category.Name,
            CategoryColor: p.Category.Color,

            ProductBatchId: batch?.ProductBatchId ?? 0,
            QuantityPerBatch: batch?.QuantityUnits ?? 0,

            SalePrice: batch?.UnitPrice ?? 0,
            ImageUrl: p.ProductImages.FirstOrDefault()?.ImagePath,
            IsFavorite: p.Favorites.Any(f => f.CompanyId == companyId),
            QuantityInCart: p.GetQuantityInCart(companyId));
    }

    public static ProductDetailResponse ToDetail(DbEntities.Product p, int? companyId)
    {
        var currentBatch = SelectBatch(p.ProductBatches, companyId);

        return new ProductDetailResponse(
            ProductId: p.ProductId,
            Name: p.Name,
            Description: p.Description,
            Weight: p.Weight,
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
                Unit: r.Ingredient.Unit.ToString())).ToList()
        );
    }

    /// <summary>
    /// Возвращает партию продукта, выбранную для текущей компании,
    /// которая уже есть в корзине, либо (если нет) партию с наименьшим объёмом поставки.
    /// </summary>
    /// <param name="batches"></param>
    /// <param name="companyId"></param>
    /// <returns>Null, если партий нет</returns>
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
