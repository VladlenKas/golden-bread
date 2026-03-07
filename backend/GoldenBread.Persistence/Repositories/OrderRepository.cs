using GoldenBread.Application.Abstractions.Data;
using GoldenBread.Application.Abstractions.Repositories;
using GoldenBread.Domain.Entities;

namespace GoldenBread.Infrastructure.Repositories;

public class OrderRepository(IGoldenBreadContext context) : IOrderRepository
{
    public async Task<Order> CreateAsync(Order order, CancellationToken cancellationToken = default)
    {
        context.Orders.Add(order);
        await context.SaveChangesAsync(cancellationToken);
        return order;
    }

    public async Task<Order?> GetByIdAsync(int orderId, CancellationToken cancellationToken = default)
    {
        return await context.Orders
            .Include(o => o.OrderItems)
                .ThenInclude(oi => oi.Batch)
                    .ThenInclude(b => b.Product)
            .Include(o => o.IngredientReservations)
            .FirstOrDefaultAsync(o => o.OrderId == orderId, cancellationToken);
    }

    public async Task UpdateAsync(Order order, CancellationToken cancellationToken = default)
    {
        context.Orders.Update(order);
        await context.SaveChangesAsync(cancellationToken);
    }
}

