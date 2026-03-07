using GoldenBread.Domain.Entities;

namespace GoldenBread.Application.Features.CompanyOrder.Services;

public interface IIngredientReservationService
{
    Task<IngredientCheckResult> CheckAsync(
       IReadOnlyList<OrderItem> orderItems,
       CancellationToken cancellationToken = default);

    Task ReserveForOrderAsync(
        IReadOnlyList<OrderItem> orderItems,
        int orderId,
        bool confirmed,
        CancellationToken cancellationToken = default);

    Task ConfirmReservationsAsync(int orderId, CancellationToken cancellationToken = default);
    Task CancelReservationsAsync(int orderId, CancellationToken cancellationToken = default);
}

public record IngredientRequirement(
    int IngredientId,
    string IngredientName,
    decimal RequiredQuantity,
    decimal AvailableQuantity,
    decimal ReservedQuantity);

public record IngredientCheckResult(
    bool IsSufficient,
    IReadOnlyList<IngredientRequirement> Requirements,
    IReadOnlyList<IngredientRequirement> Deficits);