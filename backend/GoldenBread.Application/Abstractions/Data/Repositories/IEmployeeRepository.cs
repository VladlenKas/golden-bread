using GoldenBread.Domain.Entities;

namespace GoldenBread.Application.Abstractions.Data.Repositories;

public interface IEmployeeRepository
{
    Task<IReadOnlyList<Employee>> GetActiveAsync(CancellationToken ct = default);

    Task<IReadOnlyList<Employee>> GetActiveWithTasksAsync(
        DateTime from, 
        DateTime to, 
        CancellationToken ct = default);
}
