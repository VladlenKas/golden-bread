using GoldenBread.Domain.Entities;

namespace GoldenBread.Application.Abstractions.Repositories;

public interface IIngredientReservationRepository
{
    Task CreateRangeAsync(IEnumerable<IngredientReservation> reservations, CancellationToken cancellationToken = default);
    Task<IReadOnlyList<IngredientReservation>> GetByOrderIdAsync(int orderId, CancellationToken cancellationToken = default);
    Task UpdateRangeAsync(IEnumerable<IngredientReservation> reservations, CancellationToken cancellationToken = default);
}
