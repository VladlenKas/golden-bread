using GoldenBread.Application.Abstractions.Data;
using GoldenBread.Application.Abstractions.Services;
using GoldenBread.Domain.Entities;

namespace GoldenBread.Application.Features.Product.Commands.ToggleFavorite;

public sealed class ToggleFavoriteCommandHandler(
    IGoldenBreadContext context,
    ICurrentAccountContext accountContext) :
    IRequestHandler<ToggleFavoriteCommand, Unit>
{
    public async Task<Unit> Handle(
        ToggleFavoriteCommand command,
        CancellationToken ct)
    {
        var account = await accountContext.GetAccountAsync(ct);
        int companyId = await accountContext.GetRequiredCompanyIdAsync(ct);

        var favorite = context.Favorites
            .FirstOrDefault(f =>
                f.ProductId == command.ProductId &&
                f.CompanyId == companyId);

        if (favorite == null)
        {
            context.Favorites.Add(Favorite.Create(companyId, command.ProductId));
        }
        else
        {
            context.Favorites.Remove(favorite);
        }

        await context.SaveChangesAsync(ct);

        return Unit.Value;
    }
}
