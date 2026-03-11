using GoldenBread.Domain.Entities;

namespace GoldenBread.Application.Abstractions.Repositories;

public interface IOrderRepository
{
    Task CreateAsync(
        Order order, 
        CancellationToken ct = default);

    Task<Order?> GetByIdAsync(
        int orderId, 
        CancellationToken ct = default);

    Task UpdateAsync(
        Order order, 
        CancellationToken ct = default);
}

