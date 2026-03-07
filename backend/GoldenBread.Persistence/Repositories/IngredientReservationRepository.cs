using GoldenBread.Application.Abstractions.Data;
using GoldenBread.Application.Abstractions.Repositories;
using GoldenBread.Domain.Entities;

namespace GoldenBread.Infrastructure.Repositories;

public class IngredientReservationRepository(IGoldenBreadContext context) : IIngredientReservationRepository
{
    public async Task CreateRangeAsync(
        IEnumerable<IngredientReservation> reservations,
        CancellationToken cancellationToken = default)
    {
        context.IngredientReservations.AddRange(reservations);
        await context.SaveChangesAsync(cancellationToken);
    }

    public async Task<IReadOnlyList<IngredientReservation>> GetByOrderIdAsync(
        int orderId,
        CancellationToken cancellationToken = default)
    {
        return await context.IngredientReservations
            .Where(ir => ir.OrderId == orderId)
            .ToListAsync(cancellationToken);
    }

    public async Task UpdateRangeAsync(
        IEnumerable<IngredientReservation> reservations,
        CancellationToken cancellationToken = default)
    {
        context.IngredientReservations.UpdateRange(reservations);
        await context.SaveChangesAsync(cancellationToken);
    }
}

