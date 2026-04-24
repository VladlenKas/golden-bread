namespace GoldenBread.Application.Features.CompanyOrder.Dtos;

public record CreateOrderResult
{
    public bool Success { get; init; }
    public int? OrderId { get; init; }
    public DateOnly DeliveryDate { get; init; }
    public bool IsDeferred { get; init; }
    public bool InsufficientIngredients { get; init; }
    public DateOnly? ProposedDeferredDate { get; init; }
}