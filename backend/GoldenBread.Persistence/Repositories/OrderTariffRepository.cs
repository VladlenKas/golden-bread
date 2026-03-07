using GoldenBread.Application.Abstractions.Data;
using GoldenBread.Application.Abstractions.Repositories;
using GoldenBread.Domain.Entities;

namespace GoldenBread.Infrastructure.Repositories;

public class OrderTariffRepository(IGoldenBreadContext context) : IOrderTariffRepository
{
    public async Task<OrderTariff?> GetByIdAsync(int tariffId, CancellationToken cancellationToken = default)
    {
        return await context.OrderTariffs
            .FirstOrDefaultAsync(t => t.OrderTariffId == tariffId, cancellationToken);
    }
}
