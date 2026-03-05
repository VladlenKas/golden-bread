namespace GoldenBread.Application.Features.ProductFavorites.Commands.ToggleFavorite;

public sealed record class ToggleFavoriteCommand(int ProductId) : IRequest<Unit>;
    