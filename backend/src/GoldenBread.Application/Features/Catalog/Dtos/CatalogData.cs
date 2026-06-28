using GoldenBread.Application.Abstractions.Data.Services;
using GoldenBread.Domain.Entities;

namespace GoldenBread.Application.Features.Catalog.Dtos;

public record CatalogData(
    List<Product> Products,
    List<CategoryWithCount> Categories,
    Dictionary<int, ProductSalesStatistics> SalesStatistics   // productId → stats
);