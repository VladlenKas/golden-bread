using GoldenBread.Application.Abstractions.Data.Services;
using GoldenBread.Application.Abstractions.Services;
using GoldenBread.Application.Common.Constants;
using GoldenBread.Application.Common.Exceptions;
using GoldenBread.Application.Features.Catalog.Dtos;
using GoldenBread.Application.Features.Catalog.Mapping;
using GoldenBread.Domain.Enums;

namespace GoldenBread.Application.Features.Catalog.Queires.GetProductDetail;

public sealed class GetProductDetailQueryHandler(
    ICatalogQueryService catalogQueryService,
    ICurrentAccountContext accountContext) :
    IRequestHandler<GetProductDetailQuery, ProductDetailResponse>
{
    public async Task<ProductDetailResponse> Handle(
        GetProductDetailQuery query,
        CancellationToken ct)
    {
        int? companyId = null;
        try
        {
            companyId = await accountContext.GetCompanyIdAsync(ct);
        }
        catch { }

        var product = await catalogQueryService
            .GetProductDetailAsync(query.ProductId, ct) ??
            throw new NotFoundException(ValidationErrorConstants.NotFound);

        return CatalogMapper.ToDetail(product, companyId);
    }
}
