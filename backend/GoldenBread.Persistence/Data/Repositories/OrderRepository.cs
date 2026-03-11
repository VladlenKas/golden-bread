using GoldenBread.Application.Abstractions.Data;
using GoldenBread.Application.Abstractions.Repositories;
using GoldenBread.Domain.Entities;

namespace GoldenBread.Infrastructure.Data.Repositories;

public class OrderRepository(IGoldenBreadContext context) : IOrderRepository
{
    public async Task CreateAsync(Order order, CancellationToken ct = default)
    {
        context.Orders.Add(order);
    }

    public async Task<Order?> GetByIdAsync(int orderId, CancellationToken ct = default)
    {
        return await context.Orders
            .Include(o => o.OrderItems)
                .ThenInclude(oi => oi.Batch)
                    .ThenInclude(b => b.Product)
            .Include(o => o.IngredientReservations)
            .FirstOrDefaultAsync(o => o.OrderId == orderId, ct);
    }

    public async Task UpdateAsync(Order order, CancellationToken ct = default)
    {
        context.Orders.Update(order);
    }
}

