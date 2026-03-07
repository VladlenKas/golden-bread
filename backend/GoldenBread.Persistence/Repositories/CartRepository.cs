using GoldenBread.Application.Abstractions.Data;
using GoldenBread.Application.Abstractions.Repositories;
using GoldenBread.Domain.Entities;

namespace GoldenBread.Infrastructure.Repositories;

public class CartRepository(IGoldenBreadContext context) : ICartRepository
{
    public async Task<IReadOnlyList<CartItem>> GetByCompanyIdAsync(int companyId, CancellationToken cancellationToken = default)
    {
        return await context.CartItems
            .Where(ci => ci.CompanyId == companyId)
            .Include(ci => ci.Batch)
                .ThenInclude(b => b.Product)
                    .ThenInclude(p => p.Recipes)
                        .ThenInclude(r => r.Ingredient)
            .Include(ci => ci.Company)
            .AsNoTracking()
            .ToListAsync(cancellationToken);
    }

    public async Task ClearAsync(int companyId, CancellationToken cancellationToken = default)
    {
        var items = await context.CartItems
            .Where(ci => ci.CompanyId == companyId)
            .ToListAsync(cancellationToken);

        context.CartItems.RemoveRange(items);
        await context.SaveChangesAsync(cancellationToken);
    }
}

