using GoldenBread.Domain.Entities;
using System.Linq.Expressions;

namespace GoldenBread.Application.Common.Abstractions.Repositories;

public interface ICompanyRepository
{
    Task<IEnumerable<Company?>> GetAllAsync(CancellationToken cancellationToken);
    Task<Company?> GetByIdAsync(int id, CancellationToken cancellationToken);
    Task AddAsync(Company company, CancellationToken cancellationToken);
    Task UpdateAsync(Company company, CancellationToken cancellationToken);
    Task DeleteAsync(Company company, CancellationToken cancellationToken);
    Task<bool> ExistsAsync(
        Expression<Func<Company, bool>> predicate,
        CancellationToken cancellationToken);
}
