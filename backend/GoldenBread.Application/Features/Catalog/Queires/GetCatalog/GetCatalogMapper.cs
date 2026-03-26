using GoldenBread.Application.Features.ProductCatalog.Dtos;
using GoldenBread.Domain.Entities;
using GoldenBread.Domain.Extensions;

namespace GoldenBread.Application.Features.Catalog.Queires.GetCatalog;

public class GetCatalogMapper
{
    public static ProductListItemResponse MapToResponse(DbEntities.Product p, int? companyId)
    {
        // Выбранная партия из корзины либо первая
        var batch = SelectBatch(p.ProductBatches, companyId);

        return new ProductListItemResponse(
            p.ProductId,
            p.Name,
            p.Description,
            p.ProductionTimeMinutes,
            p.CategoryId,
            p.Category.Name,
            p.Category.Color,
            batch?.ProductBatchId ?? 0,
            batch?.QuantityPerBatch ?? 0,
            batch?.UnitPrice ?? 0,
            p.ProductImages.FirstOrDefault()?.ImagePath,
            p.Favorites.Any(f => f.CompanyId == companyId),
            p.GetQuantityInCart(companyId));
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
            .OrderBy(pb => pb.QuantityPerBatch)
            .FirstOrDefault();
    }
}
