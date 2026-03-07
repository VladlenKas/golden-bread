using GoldenBread.Domain.Entities;

namespace GoldenBread.Application.Abstractions.Repositories;

public interface IEmployeeTaskRepository
{
    Task BulkCreateAsync(IEnumerable<EmployeeTask> tasks, CancellationToken cancellationToken = default);
    Task DeleteByOrderIdAsync(int orderId, CancellationToken cancellationToken = default);
}
