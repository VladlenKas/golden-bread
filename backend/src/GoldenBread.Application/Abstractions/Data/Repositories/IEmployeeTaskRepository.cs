using GoldenBread.Domain.Entities;

namespace GoldenBread.Application.Abstractions.Data.Repositories;

public interface IEmployeeTaskRepository
{
    Task BulkCreateAsync(
        IEnumerable<EmployeeTask> tasks, 
        CancellationToken ct = default);
}
