using GoldenBread.Application.Abstractions.Data;
using GoldenBread.Application.Abstractions.Data.Repositories;
using GoldenBread.Domain.Entities;

namespace GoldenBread.Infrastructure.Data.Repositories;

public class IngredientReservationRepository(IGoldenBreadContext context) : IIngredientReservationRepository
{
    public async Task<IReadOnlyList<IngredientReservation>> GetByOrderIdAsync(
        int orderId,
        CancellationToken ct = default)
    {
        return await context.IngredientReservations
            .Where(ir => ir.OrderId == orderId)
            .ToListAsync(ct);
    }

    public async Task CreateRangeAsync(
        IEnumerable<IngredientReservation> reservations,
        CancellationToken ct = default)
    {
        context.IngredientReservations.AddRange(reservations);
    }

    public async Task UpdateRangeAsync(
        IEnumerable<IngredientReservation> reservations,
        CancellationToken ct = default)
    {
        context.IngredientReservations.UpdateRange(reservations);
    }
}

