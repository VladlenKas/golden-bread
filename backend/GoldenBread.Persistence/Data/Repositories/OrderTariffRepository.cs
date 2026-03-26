using GoldenBread.Application.Abstractions.Data;
using GoldenBread.Application.Abstractions.Data.Repositories;
using GoldenBread.Domain.Entities;

namespace GoldenBread.Infrastructure.Data.Repositories;

public class OrderTariffRepository(IGoldenBreadContext context) : IOrderTariffRepository
{
    public async Task<OrderTariff?> GetByIdAsync(int tariffId, CancellationToken ct = default)
    {
        return await context.OrderTariffs
            .FirstOrDefaultAsync(t => t.OrderTariffId == tariffId, ct);
    }
}
