using GoldenBread.Application.Abstractions.Data.Repositories;
using GoldenBread.Application.Abstractions.Services;

namespace GoldenBread.Application.Features.Favorites.Commands.ToggleFavorite;

public sealed class ToggleFavoriteCommandHandler(
    IFavoriteRepository favoriteRepository,
    ICurrentAccountContext accountContext) :
    IRequestHandler<ToggleFavoriteCommand, Unit>
{
    public async Task<Unit> Handle(
        ToggleFavoriteCommand command,
        CancellationToken ct)
    {
        int companyId = await accountContext.GetRequiredCompanyIdAsync(ct);

        await favoriteRepository.ToggleAsync(command.ProductId, companyId, ct);

        return Unit.Value;
    }
}

