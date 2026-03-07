using GoldenBread.Application.Abstractions.Data;
using GoldenBread.Application.Abstractions.Repositories;
using GoldenBread.Domain.Entities;

namespace GoldenBread.Infrastructure.Repositories;

public class OrderItemRepository(IGoldenBreadContext context) : IOrderItemRepository
{
    public async Task CreateRangeAsync(IEnumerable<OrderItem> orderItems, CancellationToken cancellationToken = default)
    {
        context.OrderItems.AddRange(orderItems);
        await context.SaveChangesAsync(cancellationToken);
    }

    public async Task<IReadOnlyList<OrderItem>> GetByOrderIdAsync(int orderId, CancellationToken cancellationToken = default)
    {
        return await context.OrderItems
            .Where(oi => oi.OrderId == orderId)
            .Include(oi => oi.Batch)
                .ThenInclude(b => b.Product)
            .Include(oi => oi.EmployeeTasks)
            .ToListAsync(cancellationToken);
    }
}

