using GoldenBread.Domain.Entities;

namespace GoldenBread.Application.Features.CompanyOrder.Services;

public interface IIngredientReservationService
{
    Task<bool> CheckAsync(
       IReadOnlyList<OrderItem> orderItems,
       CancellationToken ct = default);

    Task ReserveForOrderAsync(
        IReadOnlyList<OrderItem> orderItems,
        int orderId,
        CancellationToken ct = default);

    Task ConfirmReservationsAsync(int orderId, CancellationToken ct = default);
    Task CancelReservationsAsync(int orderId, CancellationToken ct = default);
}
