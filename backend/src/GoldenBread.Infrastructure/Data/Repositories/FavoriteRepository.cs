using GoldenBread.Application.Abstractions.Data.Repositories;
using GoldenBread.Application.Abstractions.Data;
using GoldenBread.Domain.Entities;

namespace GoldenBread.Infrastructure.Data.Repositories;

public sealed class FavoriteRepository(IGoldenBreadContext context) : IFavoriteRepository
{
    public async Task ToggleAsync(
        int productId, 
        int companyId,
        CancellationToken ct = default)
    {
        var favorite = await context.Favorites
            .FirstOrDefaultAsync(f =>
                f.ProductId == productId &&
                f.CompanyId == companyId, ct);

        if (favorite == null)
            context.Favorites.Add(Favorite.Create(companyId, productId));
        else
            context.Favorites.Remove(favorite);
    }
}
