namespace GoldenBread.Application.Features.Product.Commands.ToggleFavorite;

public sealed record class ToggleFavoriteCommand(int ProductId) : IRequest<Unit>;
