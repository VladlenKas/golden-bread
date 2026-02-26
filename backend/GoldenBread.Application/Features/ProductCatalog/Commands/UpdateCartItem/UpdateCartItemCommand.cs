namespace GoldenBread.Application.Features.ProductCatalog.Commands.UpdateCartItem;

public sealed record class UpdateCartItemCommand(
    int ProductId, 
    int ProductBatchId, 
    int Quantity) : IRequest<int>;
