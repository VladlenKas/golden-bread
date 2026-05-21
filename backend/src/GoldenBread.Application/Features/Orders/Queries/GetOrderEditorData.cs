using GoldenBread.Application.Abstractions.Data;
using GoldenBread.Application.Features.Orders.Dtos;

namespace GoldenBread.Application.Features.Orders.Queries;

public sealed record GetOrderEditorDataQuery : IRequest<OrderEditorDataResponse>;

public sealed class GetOrderEditorDataQueryHandler(IGoldenBreadContext context)
    : IRequestHandler<GetOrderEditorDataQuery, OrderEditorDataResponse>
{
    public async Task<OrderEditorDataResponse> Handle(
        GetOrderEditorDataQuery query,
        CancellationToken ct)
    {
        var companies = await context.Companies
            .AsNoTracking()
            .Include(c => c.Account)
            .Where(c => c.Account.DeletedAt == null)
            .Select(c => new CompanyLookup(c.CompanyId, c.Name))
            .ToListAsync(ct);

        var products = await context.Products
            .AsNoTracking()
            .Where(p => p.DeletedAt == null)
            .Include(p => p.ProductBatches)
            .Select(p => new ProductEditorDto(
                p.ProductId,
                p.Name,
                p.CostPrice,
                p.ProductBatches.Select(b => new ProductBatchEditorDto(
                    b.ProductBatchId,
                    b.MarkupPercent,
                    b.QuantityUnits)).ToList()))
            .ToListAsync(ct);

        return new OrderEditorDataResponse(companies, products);
    }
}