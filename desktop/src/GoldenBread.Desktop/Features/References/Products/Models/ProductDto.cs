namespace GoldenBread.Desktop.Features.References.Products.Models;

public record class ProductDto(
    int ProductId,
    string Name,
    string Description,
    decimal CostPrice,
    decimal Weight,
    int ProductionTimeMinutes,
    int ShelfLifeDays,
    decimal StorageTempMin,
    decimal StorageTempMax,
    int CategoryId);
