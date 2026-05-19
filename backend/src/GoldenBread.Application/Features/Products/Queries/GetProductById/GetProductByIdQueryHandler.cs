using GoldenBread.Application.Abstractions.Data.Repositories;
using GoldenBread.Application.Features.Products.Dtos;

namespace GoldenBread.Application.Features.Products.Queries.GetProductById;

public sealed class GetProductByIdQueryHandler(IProductRepository repository)
    : IRequestHandler<GetProductByIdQuery, ProductDto?>
{
    public async Task<ProductDto?> Handle(GetProductByIdQuery query, CancellationToken ct)
    {
        var p = await repository.GetByIdAsync(query.Id, ct);
        if (p is null) return null;

        return new ProductDto(
            p.ProductId, p.Name, p.Description, p.CostPrice, p.Weight,
            p.ProductionTimeMinutes, p.ShelfLifeDays,
            p.StorageTempMin, p.StorageTempMax, p.CategoryId);
    }
}