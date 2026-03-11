using GoldenBread.Domain.Entities;

namespace GoldenBread.Application.Abstractions.Repositories;

public interface IIngredientReservationRepository
{
    Task<IReadOnlyList<IngredientReservation>> GetByOrderIdAsync(
        int orderId, 
        CancellationToken ct = default);

    Task CreateRangeAsync(
        IEnumerable<IngredientReservation> reservations, 
        CancellationToken ct = default);

    Task UpdateRangeAsync(
        IEnumerable<IngredientReservation> reservations, 
        CancellationToken ct = default);
}
