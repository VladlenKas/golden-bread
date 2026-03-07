using GoldenBread.Domain.Entities;

namespace GoldenBread.Application.Abstractions.Repositories;

public interface IOrderRepository
{
    Task<Order> CreateAsync(Order order, CancellationToken cancellationToken = default);
    Task<Order?> GetByIdAsync(int orderId, CancellationToken cancellationToken = default);
    Task UpdateAsync(Order order, CancellationToken cancellationToken = default);
}

