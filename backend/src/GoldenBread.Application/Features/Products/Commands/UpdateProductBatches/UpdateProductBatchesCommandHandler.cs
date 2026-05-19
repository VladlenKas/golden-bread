using GoldenBread.Application.Abstractions.Data;
using GoldenBread.Application.Abstractions.Data.Repositories;
using GoldenBread.Domain.Entities;

namespace GoldenBread.Application.Features.Products.Commands.UpdateProductBatches;

public sealed class UpdateProductBatchesCommandHandler(
    IGoldenBreadContext context,
    IProductRepository repository,
    IProductBatchRepository batchRepository,
    IUnitOfWork unitOfWork)
    : IRequestHandler<UpdateProductBatchesCommand, bool>
{
    public async Task<bool> Handle(UpdateProductBatchesCommand request, CancellationToken ct)
    {
        var product = await repository.GetByIdAsync(request.ProductId, ct);
        if (product is null) return false;

        var existing = product.ProductBatches.ToList();
        var incoming = request.Batches;

        // Delete missing
        foreach (var e in existing)
        {
            if (!incoming.Any(i => i.ProductBatchId == e.ProductBatchId))
                context.ProductBatches.Remove(e);
        }

        // Update existing
        foreach (var i in incoming.Where(i => i.ProductBatchId.HasValue))
        {
            var e = existing.FirstOrDefault(x => x.ProductBatchId == i.ProductBatchId);
            if (e != null) e.Update(i.MarkupPercent, i.QuantityUnits);
        }

        // Add new
        foreach (var i in incoming.Where(i => !i.ProductBatchId.HasValue))
        {
            await batchRepository.AddAsync(ProductBatch.Create(
                request.ProductId, i.QuantityUnits, i.MarkupPercent), ct);
        }

        await unitOfWork.SaveChangesAsync(ct);
        return true;
    }
}