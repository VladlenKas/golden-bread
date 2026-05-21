using GoldenBread.Application.Abstractions.Data;
using GoldenBread.Application.Abstractions.Data.Repositories;
using GoldenBread.Application.Common.Exceptions;

namespace GoldenBread.Application.Features.Products.Commands.UpdateProduct;

public sealed class UpdateProductCommandHandler(
    IProductRepository repository,
    IUnitOfWork unitOfWork)
    : IRequestHandler<UpdateProductCommand, bool>
{
    public async Task<bool> Handle(UpdateProductCommand request, CancellationToken ct)
    {
        var dto = request.Dto;
        var product = await repository.GetByIdAsync(dto.ProductId, ct);

        if (product is null) return false;

        if (await repository.ExistsByNameAsync(dto.Name, dto.ProductId, ct))
            throw new DuplicateEntityException(nameof(dto.Name));

        product.Update(
            dto.Name, dto.Description, dto.CostPrice, dto.Weight,
            dto.ProductionTimeMinutes, dto.ShelfLifeDays,
            dto.StorageTempMin, dto.StorageTempMax, dto.CategoryId);

        await unitOfWork.SaveChangesAsync(ct);
        return true;
    }
}