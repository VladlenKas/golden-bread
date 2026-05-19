using GoldenBread.Application.Abstractions.Data;
using GoldenBread.Application.Abstractions.Data.Repositories;
using GoldenBread.Application.Abstractions.Services;
using GoldenBread.Domain.Entities;


namespace GoldenBread.Application.Features.Products.Commands.UpdateProductImages;

public sealed class UpdateProductImagesCommandHandler(
    IGoldenBreadContext context,
    IProductRepository repository,
    IFileStorage fileStorage,
    IUnitOfWork unitOfWork)
    : IRequestHandler<UpdateProductImagesCommand, bool>
{
    public async Task<bool> Handle(UpdateProductImagesCommand request, CancellationToken ct)
    {
        var product = await repository.GetByIdAsync(request.ProductId, ct);
        if (product is null) return false;

        var existing = product.ProductImages.ToList();
        var incomingPaths = request.ImagePaths;

        // Delete removed images (from DB and disk)
        foreach (var e in existing)
        {
            if (!incomingPaths.Contains(e.ImagePath))
            {
                try { await fileStorage.DeleteAsync(e.ImagePath); } catch { }
                context.ProductImages.Remove(e);
            }
        }

        // Add new
        foreach (var path in incomingPaths.Where(p => !existing.Any(e => e.ImagePath == p)))
        {
            context.ProductImages.Add(new ProductImage
            {
                ProductId = request.ProductId,
                ImagePath = path
            });
        }

        await unitOfWork.SaveChangesAsync(ct);
        return true;
    }
}