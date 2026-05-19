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
        var account = await accountContext.GetAccountAsync(ct);
        int? companyId = null;

        if (account != null && account.AccountType == AccountType.Company)
            companyId = await accountContext.GetCompanyIdAsync(ct);

        var product = await catalogQueryService
            .GetProductDetailAsync(query.ProductId, ct) ??
            throw new NotFoundException(ValidationErrorConstants.NotFound);

        return CatalogMapper.ToDetail(product, companyId);
    }
}
