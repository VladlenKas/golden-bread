using GoldenBread.Application.Abstractions.Data;
using GoldenBread.Application.Abstractions.Services;
using GoldenBread.Domain.Entities;

namespace GoldenBread.Application.Features.ProductFavorites.Commands.ToggleFavorite;

public sealed class ToggleFavouriteCommandHandle(
    IGoldenBreadContext context,
    ICurrentAccountContext accountContext) : 
    IRequestHandler<ToggleFavoriteCommand, Unit>
{
    public async Task<Unit> Handle(
        ToggleFavoriteCommand command, 
        CancellationToken ct)
    {
        var account = await accountContext.GetAccountAsync(ct);
        int companyId = account.Company.CompanyId;

        var favourite = context.Favorites
            .FirstOrDefault(f =>
                f.ProductId == command.ProductId &&
                f.CompanyId == companyId);

        if (favourite == null)
            context.Favorites.Add(Favorite.Create(companyId, command.ProductId));
        else
            context.Favorites.Remove(favourite);

        await context.SaveChangesAsync(ct);

        return Unit.Value;  
    }
}
