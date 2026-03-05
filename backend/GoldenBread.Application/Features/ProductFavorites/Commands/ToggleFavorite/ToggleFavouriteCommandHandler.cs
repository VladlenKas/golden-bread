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
        CancellationToken cancellationToken)
    {
        var account = await accountContext.GetAccountAsync(cancellationToken);
        int companyId = account.Company.CompanyId;

        var favourite = context.Favourites
            .FirstOrDefault(f =>
                f.ProductId == command.ProductId &&
                f.CompanyId == companyId);

        if (favourite == null)
            context.Favourites.Add(Favorite.Create(companyId, command.ProductId));
        else
            context.Favourites.Remove(favourite);

        await context.SaveChangesAsync(cancellationToken);

        return Unit.Value;  
    }
}
