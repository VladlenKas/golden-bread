using GoldenBread.Application.Features.Products.Dtos;

namespace GoldenBread.Application.Features.Products.Commands.UpdateProductBatches;

public record UpdateProductBatchesCommand(int ProductId, List<ProductBatchDto> Batches) : IRequest<bool>;
