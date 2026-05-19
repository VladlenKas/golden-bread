namespace GoldenBread.Desktop.Features.References.Products.Models;

public record UpdateProductBatchesCommand(int ProductId, List<ProductBatchDto> Batches);