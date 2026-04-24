using GoldenBread.Application.Features.Catalog.Dtos;

namespace GoldenBread.Application.Features.Favorites.Queries.GetFavorites;

public sealed record GetFavoritesQuery : IRequest<List<ProductListItemResponse>>;

