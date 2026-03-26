using GoldenBread.Domain.Entities;

namespace GoldenBread.Application.Abstractions.Data.Repositories;

public interface IEmployeeTaskRepository
{
    Task BulkCreateAsync(
        IEnumerable<EmployeeTask> tasks, 
        CancellationToken ct = default);

    Task DeleteByOrderIdAsync(
        int orderId, 
        CancellationToken ct = default);
}
