using GoldenBread.Application.Abstractions.Data;
using GoldenBread.Application.Abstractions.Data.Repositories;
using GoldenBread.Application.Common.Exceptions;

namespace GoldenBread.Application.Features.ProductCategory.Commands.CreateProductCategory;

public sealed class CreateProductCategoryCommandHandler(
    ICategoryRepository repository,
    IUnitOfWork unitOfWork)
    : IRequestHandler<CreateProductCategoryCommand, int>
{
    public async Task<int> Handle(CreateProductCategoryCommand request, CancellationToken ct)
    {
        var dto = request.Dto;

        if (await repository.ExistsByNameAsync(dto.Name, null, ct))
            throw new DuplicateEntityException(nameof(dto.Name));

        var entity = new DbEntities.ProductCategory
        {
            Name = dto.Name,
            Color = dto.Color
        };
        await repository.AddAsync(entity, ct);
        await unitOfWork.SaveChangesAsync(ct);
        return entity.ProductCategoryId;
    }
}