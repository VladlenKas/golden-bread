using GoldenBread.Domain.Entities;

namespace GoldenBread.Application.Abstractions.Repositories;

public interface IOrderItemRepository
{
    Task CreateRangeAsync(
        IEnumerable<OrderItem> orderItems, 
        CancellationToken ct = default);

    Task<IReadOnlyList<OrderItem>> GetByOrderIdAsync(
        int orderId, 
        CancellationToken ct = default);
}
