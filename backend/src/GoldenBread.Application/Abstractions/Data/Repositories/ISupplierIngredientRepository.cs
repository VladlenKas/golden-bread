using GoldenBread.Domain.Entities;

namespace GoldenBread.Application.Abstractions.Data.Repositories;

public interface ISupplierIngredientRepository
{
    Task<IReadOnlyList<SupplierIngredient>> GetAllAsync(CancellationToken ct = default);
    Task<SupplierIngredient?> GetByIdAsync(int id, CancellationToken ct = default);
    Task<bool> ExistsByNameAsync(int supplierId, string name, int? excludeId = null, CancellationToken ct = default);
    Task AddAsync(SupplierIngredient entity, CancellationToken ct = default);
}
