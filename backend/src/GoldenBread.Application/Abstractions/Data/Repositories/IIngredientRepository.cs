using GoldenBread.Domain.Entities;

namespace GoldenBread.Application.Abstractions.Data.Repositories;

public interface IIngredientRepository
{
    Task<IReadOnlyList<Ingredient>> GetAllAsync(CancellationToken ct = default);
    Task<Ingredient?> GetByIdAsync(int id, CancellationToken ct = default);
    Task<bool> ExistsByNameAsync(string name, int? excludeId = null, CancellationToken ct = default);
    Task AddAsync(Ingredient ingredient, CancellationToken ct = default);
}
