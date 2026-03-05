using GoldenBread.Application.Abstractions.Data;
using GoldenBread.Application.Abstractions.Services;
using GoldenBread.Application.Features.ProductCatalog.Dtos;
using GoldenBread.Domain.Entities;
using GoldenBread.Domain.Extensions;

namespace GoldenBread.Application.Features.ProductCatalog.Queires.GetCatalog;

public sealed class GetCatalogQueryHandler(
    IGoldenBreadContext context,
    ICurrentAccountContext accountContext) :
    IRequestHandler<GetCatalogQuery, CatalogResponse>
{
    public async Task<CatalogResponse> Handle(
        GetCatalogQuery query, 
        CancellationToken cancellationToken)
    {
        int companyId = await accountContext.GetCompanyIdAsync(cancellationToken);

        var productsList = await context.Products
            .AsNoTracking()
            .Include(p => p.Category)      
            .Include(p => p.ProductImages)  
            .Include(p => p.ProductBatches) 
                .ThenInclude(pb => pb.CartItems)  
            .Include(p => p.Favourites)    
            .Select(p => MapToResponse(p, companyId))
            .ToListAsync(cancellationToken);

        var categoriesList = await context.ProductCategories
            .AsNoTracking()
            .Select(c => new ProductCategoryResponse
            {
                ProductCategoryId = c.ProductCategoryId,
                Name = c.Name,
                Color = c.Color,
                ProductCount = c.Products.Count
            })
            .ToListAsync(cancellationToken);

        return new CatalogResponse
        {
            ProductsList = productsList,
            Categories = categoriesList
        };
    }

    private static ProductListItemResponse MapToResponse(Product p, int companyId)
    {
        // Активные партии
        var batches = p.ProductBatches
            .ToList();

        // Ищем выбранный в корзине
        var cartBatch = batches
            .FirstOrDefault(pb => pb.CartItems
                .Any(ci => ci.CompanyId == companyId));

        // Или минимальный
        var batch = cartBatch ?? batches
            .OrderBy(pb => pb.QuantityPerBatch)
            .FirstOrDefault();

        return new ProductListItemResponse
        {
            ProductId = p.ProductId,
            Name = p.Name,
            CategoryName = p.Category.Name,
            CategoryColor = p.Category.Color,
            ImageUrl = p.ProductImages.Select(i => i.ImagePath).FirstOrDefault(),
            ProductBatchId = batch?.ProductBatchId ?? 0,
            QuantityPerBatch = batch?.QuantityPerBatch ?? 0,
            SalePrice = batch?.UnitPrice ?? 0,
            IsFavorite = p.Favourites.Any(f => f.CompanyId == companyId),
            QuantityInCart = p.GetQuantityInCart(companyId)
        };
    }
}
        