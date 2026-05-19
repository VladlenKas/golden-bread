using GoldenBread.Application.Abstractions.Data.Repositories;
using GoldenBread.Application.Features.ProductCategory.Dtos;

namespace GoldenBread.Application.Features.ProductCategory.Queries.GetProductCategoryById;

public sealed class GetProductCategoryByIdQueryHandler(ICategoryRepository repository)
    : IRequestHandler<GetProductCategoryByIdQuery, ProductCategoryDto?>
{
    public async Task<ProductCategoryDto?> Handle(GetProductCategoryByIdQuery query, CancellationToken ct)
    {
        var e = await repository.GetByIdAsync(query.Id, ct);
        if (e is null) return null;
        return new ProductCategoryDto(e.ProductCategoryId, e.Name, e.Color);
    }
}