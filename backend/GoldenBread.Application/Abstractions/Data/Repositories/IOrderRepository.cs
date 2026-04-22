using GoldenBread.Domain.Entities;

namespace GoldenBread.Application.Abstractions.Data.Repositories;

public interface IOrderRepository
{
    Task CreateAsync(
        Order order, 
        CancellationToken ct = default);
    Task<List<Order>> GetAllByCompanyIdAsync(
        int companyId, 
        CancellationToken ct = default);

    Task<Order?> GetByIdAsync(
        int orderId, 
        CancellationToken ct = default);

    Task UpdateAsync(
        Order order, 
        CancellationToken ct = default);
}

