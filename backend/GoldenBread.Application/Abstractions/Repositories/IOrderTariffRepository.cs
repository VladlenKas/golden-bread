using GoldenBread.Domain.Entities;

namespace GoldenBread.Application.Abstractions.Repositories;

public interface IOrderTariffRepository
{
    Task<OrderTariff?> GetByIdAsync(
        int tariffId, 
        CancellationToken ct = default);
}
