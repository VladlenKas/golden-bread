using GoldenBread.Application.Abstractions.Data;
using GoldenBread.Application.Abstractions.Data.Repositories;
using GoldenBread.Application.Common.Exceptions;

namespace GoldenBread.Application.Features.ProductCategory.Commands.UpdateProductCategory;

public sealed class UpdateProductCategoryCommandHandler(
    ICategoryRepository repository,
    IUnitOfWork unitOfWork)
    : IRequestHandler<UpdateProductCategoryCommand, bool>
{
    public async Task<bool> Handle(UpdateProductCategoryCommand request, CancellationToken ct)
    {
        var dto = request.Dto;

        if (await repository.ExistsByNameAsync(dto.Name, dto.ProductCategoryId, ct))
            throw new DuplicateEntityException(nameof(dto.Name));

        var entity = await repository.GetByIdAsync(dto.ProductCategoryId, ct);
        if (entity is null) return false;

        entity.Name = dto.Name;
        entity.Color = dto.Color;

        await unitOfWork.SaveChangesAsync(ct);
        return true;
    }
}