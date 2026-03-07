using GoldenBread.Domain.Entities;

namespace GoldenBread.Application.Abstractions.Repositories;

public interface ICartRepository
{
    Task<IReadOnlyList<CartItem>> GetByCompanyIdAsync(int companyId, CancellationToken cancellationToken = default);
    Task ClearAsync(int companyId, CancellationToken cancellationToken = default);
}
