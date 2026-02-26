namespace GoldenBread.Application.Features.ProductCatalog.Commands.ToggleFavorite;

public sealed record class ToggleFavoriteCommand(int ProductId) : IRequest<Unit>;
    