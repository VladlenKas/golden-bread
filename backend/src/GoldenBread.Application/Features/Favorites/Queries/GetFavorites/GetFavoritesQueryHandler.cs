using GoldenBread.Application.Abstractions.Data.Services;
using GoldenBread.Application.Abstractions.Services;
using GoldenBread.Application.Features.Catalog.Dtos;
using GoldenBread.Application.Features.Catalog.Mapping;

namespace GoldenBread.Application.Features.Favorites.Queries.GetFavorites;

public sealed class GetFavoritesQueryHandler(
    ICatalogQueryService catalogQuery,
    ICurrentAccountContext accountContext) :
    IRequestHandler<GetFavoritesQuery, List<ProductListItemResponse>>
{
    public async Task<List<ProductListItemResponse>> Handle(
        GetFavoritesQuery query,
        CancellationToken ct)
    {
        int? companyId = await accountContext.GetCompanyIdAsync(ct);
        var data = await catalogQuery.GetCatalogAsync(companyId, ct);

        var favoritesListResponse = data.Products
            .Select(p => CatalogMapper.ToListItems(p, companyId))
            .Where(i => i.IsFavorite == true)
            .ToList();

        return favoritesListResponse;
    }
}
