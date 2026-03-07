using GoldenBread.Application.Abstractions.Data;
using GoldenBread.Application.Abstractions.Repositories;
using GoldenBread.Domain.Entities;

namespace GoldenBread.Infrastructure.Repositories;

public class EmployeeRepository(IGoldenBreadContext context) : IEmployeeRepository
{
    public async Task<IReadOnlyList<Employee>> GetActiveAsync(CancellationToken cancellationToken = default)
    {
        return await context.Employees
            .Where(e => e.DeletedAt == null)
            .AsNoTracking()
            .ToListAsync(cancellationToken);
    }

    public async Task<IReadOnlyList<Employee>> GetActiveWithTasksAsync(
        DateTime from,
        DateTime to,
        CancellationToken cancellationToken = default)
    {
        return await context.Employees
            .Where(e => e.DeletedAt == null)
            .Include(e => e.EmployeeTasks
                .Where(et => et.EndTime > from && et.StartTime < to))
            .AsNoTracking()
            .ToListAsync(cancellationToken);
    }
}

