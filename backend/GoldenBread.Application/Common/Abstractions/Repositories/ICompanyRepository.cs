using GoldenBread.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;

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
