using GoldenBread.Domain.Entities;

namespace GoldenBread.Application.Abstractions.Repositories;

public interface IEmployeeRepository
{
    Task<IReadOnlyList<Employee>> GetActiveAsync(CancellationToken cancellationToken = default);
    Task<IReadOnlyList<Employee>> GetActiveWithTasksAsync(DateTime from, DateTime to, CancellationToken cancellationToken = default);
}
