using GoldenBread.Application.Abstractions.Data;
using GoldenBread.Application.Abstractions.Data.Repositories;

namespace GoldenBread.Application.Features.ProductCategory.Commands.DeleteProductCategory;

public sealed class DeleteProductCategoryCommandHandler(
    ICategoryRepository repository,
    IUnitOfWork unitOfWork)
    : IRequestHandler<DeleteProductCategoryCommand, bool>
{
    public async Task<bool> Handle(DeleteProductCategoryCommand request, CancellationToken ct)
    {
        var entity = await repository.GetByIdAsync(request.ProductCategoryId, ct);
        if (entity is null) return false;
        if (entity.Products.Any(p => p.DeletedAt == null))
            throw new InvalidOperationException("Category has active products");

        entity.DeletedAt = DateTime.UtcNow;
        await unitOfWork.SaveChangesAsync(ct);
        return true;
    }
}