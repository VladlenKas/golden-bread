namespace GoldenBread.Application.Features.ProductCatalog.Commands.ToggleFavourite;

public sealed record class ToggleFavouriteCommand(int ProductId) : IRequest<Unit>;
    