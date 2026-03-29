using GoldenBread.Application.Abstractions.Data;
using GoldenBread.Application.Abstractions.Data.Repositories;
using GoldenBread.Domain.Entities;

namespace GoldenBread.Infrastructure.Data.Repositories;

public class CartRepository(IGoldenBreadContext context) : ICartRepository
{
    public async Task<IReadOnlyList<CartItem>> GetByCompanyIdAsync(int companyId, CancellationToken ct = default)
    {
        return await context.CartItems
            .Where(ci => ci.CompanyId == companyId)
            .Include(ci => ci.Batch)
                .ThenInclude(b => b.Product)
                    .ThenInclude(p => p.Recipes)
                        .ThenInclude(r => r.Ingredient)
            .Include(ci => ci.Company)
            .AsNoTracking()
            .ToListAsync(ct);
    }

    public async Task ClearAsync(int companyId, CancellationToken ct = default)
    {
        var items = await context.CartItems
            .Where(ci => ci.CompanyId == companyId)
            .ToListAsync(ct);

        context.CartItems.RemoveRange(items);
    }

    public Task Get(CancellationToken ct = default)
    {
        throw new NotImplementedException();
    }
}

