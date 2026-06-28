namespace GoldenBread.Application.Features.Catalog.Dtos;

public record ProductSalesStatistics(
    int TotalSoldAllTime,
    IReadOnlyList<SeasonalSalesData> SeasonalSales
);