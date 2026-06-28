using GoldenBread.Domain.Enums;

namespace GoldenBread.Application.Features.Catalog.Dtos;

public record SeasonalSalesData(
    Season Season,
    int Year,
    int TotalUnitsSold
);