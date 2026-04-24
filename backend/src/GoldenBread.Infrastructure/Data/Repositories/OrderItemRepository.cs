using GoldenBread.Application.Abstractions.Data;
using GoldenBread.Application.Abstractions.Data.Repositories;
using GoldenBread.Domain.Entities;

namespace GoldenBread.Infrastructure.Data.Repositories;

public class OrderItemRepository(IGoldenBreadContext context) : IOrderItemRepository
{
    public async Task CreateRangeAsync(IEnumerable<OrderItem> orderItems, CancellationToken ct = default)
    {
        context.OrderItems.AddRange(orderItems);
    }

    public async Task<IReadOnlyList<OrderItem>> GetByOrderIdAsync(int orderId, CancellationToken ct = default)
    {
        return await context.OrderItems
            .Where(oi => oi.OrderId == orderId)
            .Include(oi => oi.Batch)
                .ThenInclude(b => b.Product)
            .Include(oi => oi.EmployeeTasks)
            .ToListAsync(ct);
    }
}

