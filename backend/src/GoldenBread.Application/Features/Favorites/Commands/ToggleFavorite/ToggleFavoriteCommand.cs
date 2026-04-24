namespace GoldenBread.Application.Features.Favorites.Commands.ToggleFavorite;

public sealed record ToggleFavoriteCommand(int ProductId) 
    : IRequest<Unit>;
