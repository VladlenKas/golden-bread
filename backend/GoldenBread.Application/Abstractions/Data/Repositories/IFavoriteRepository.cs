using GoldenBread.Domain.Entities;

namespace GoldenBread.Application.Abstractions.Data.Repositories;

public interface IFavoriteRepository
{
    Task ToggleAsync(int productId, int companyId, CancellationToken ct);
}
