using GoldenBread.Domain.Entities;

namespace GoldenBread.Application.Abstractions.Repositories;

public interface ICartRepository
{
    Task<IReadOnlyList<CartItem>> GetByCompanyIdAsync(
        int companyId, 
        CancellationToken ct = default);

    Task ClearAsync(
        int companyId, 
        CancellationToken ct = default);
}
