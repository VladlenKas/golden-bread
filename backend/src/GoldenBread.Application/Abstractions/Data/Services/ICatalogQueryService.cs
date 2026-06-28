using GoldenBread.Application.Features.Catalog.Dtos;
using GoldenBread.Domain.Entities;

namespace GoldenBread.Application.Abstractions.Data.Services;

public interface ICatalogQueryService
{
    Task<CatalogData> GetCatalogAsync(int? companyId, CancellationToken ct);
    Task<Product?> GetProductDetailAsync(int productId, CancellationToken ct);
}

public sealed record CategoryWithCount(
    int ProductCategoryId,
    string Name,
    string Color,
    int ProductsCount);
