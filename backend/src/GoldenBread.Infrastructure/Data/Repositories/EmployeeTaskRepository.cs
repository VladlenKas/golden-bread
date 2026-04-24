using GoldenBread.Application.Abstractions.Data;
using GoldenBread.Application.Abstractions.Data.Repositories;
using GoldenBread.Domain.Entities;

namespace GoldenBread.Infrastructure.Data.Repositories;

public class EmployeeTaskRepository(IGoldenBreadContext context) : IEmployeeTaskRepository
{
    public async Task BulkCreateAsync(
        IEnumerable<EmployeeTask> tasks, 
        CancellationToken ct = default)
    {
        context.EmployeeTasks.AddRange(tasks);
    }
}

