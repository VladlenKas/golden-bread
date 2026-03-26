using GoldenBread.Application.Abstractions.Data;
using GoldenBread.Application.Abstractions.Services;
using GoldenBread.Application.Features.ProductCatalog.Dtos;
using GoldenBread.Domain.Extensions;

namespace GoldenBread.Application.Features.ProductCatalog.Queires.GetProductDetail;

public sealed class GetProductDetailQueryHandler(
    IGoldenBreadContext context,
    ICurrentAccountContext accountContext) :
    IRequestHandler<GetProductDetailQuery, ProductDetailResponse>
{
    public async Task<ProductDetailResponse> Handle(
        GetProductDetailQuery query, 
        CancellationToken ct)
    {
        int? companyId = await accountContext.GetCompanyIdAsync(ct);

        var productBatches = await context.ProductBatches
            .AsNoTracking()
            .Where(p => p.ProductId == query.ProductId)
            .Include(pb => pb.Product)
            .Select(pb => new ProductBatchResponse
            {
                ProductBatchId = pb.ProductBatchId,
                QuantityPerBatch = pb.QuantityPerBatch,
                UnitPrice = pb.UnitPrice,
                TotalPrice = pb.TotalPrice
            }).ToListAsync(ct);

        var productDetail = await context.Products
            .AsNoTracking()
            .Where(p => p.ProductId == query.ProductId)
            .Include(p => p.ProductBatches)
                .ThenInclude(pb => pb.CartItems)
            .Select(p => new ProductDetailResponse
            {
                ProductId = p.ProductId,
                Name = p.Name,
                Description = p.Description,
                Weight = p.Weight,
                ProductionTimeMinutes = p.ProductionTimeMinutes,
                ShelfLifeDays = p.ShelfLifeDays,
                StorageTempMin = p.StorageTempMin,
                StorageTempMax = p.StorageTempMax,

                // Категория
                CategoryId = p.CategoryId,
                CategoryName = p.Category.Name,
                CategoryColor = p.Category.Color,

                // Изображения
                ImageUrls = p.ProductImages
                    .Select(i => i.ImagePath).ToList(),

                // Доступные партии
                CurrentBatchId = p.ProductBatches
                    .SelectMany(pb => pb.CartItems
                        .Where(ci => ci.CompanyId == companyId))
                    .Select(ci => ci.BatchId)
                    .FirstOrDefault(),

                // Флаги / Вычисляемые свойства
                QuantityInCart = p.GetQuantityInCart(companyId),
                TotalCostInCart = p.GetTotalCostInCart(companyId),
                IsFavorite = p.Favorites
                    .Any(f => f.CompanyId == companyId),

                AvailableBatches = productBatches,

                // Рецепт
                Ingredients = p.Recipes
                    .Select(r => new IngredientResponse
                {
                    IngredientId = r.Ingredient.IngredientId,
                    Name = r.Ingredient.Name,
                    Quantity = r.Quantity,
                    Unit = r.Ingredient.Unit.ToString()
                }).ToList()
            })
            .FirstAsync(ct);

        return productDetail;
    }
}
