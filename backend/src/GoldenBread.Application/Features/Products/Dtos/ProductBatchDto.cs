namespace GoldenBread.Application.Features.Products.Dtos;

public record ProductBatchDto(
    int? ProductBatchId,
    int ProductId,
    int MarkupPercent,
    int QuantityUnits);