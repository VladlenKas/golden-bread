using GoldenBread.Application.Abstractions.Data;
using GoldenBread.Application.Abstractions.Data.Repositories;
using GoldenBread.Domain.Entities;

namespace GoldenBread.Infrastructure.Data.Repositories;

public class EmployeeRepository(IGoldenBreadContext context) : IEmployeeRepository
{
    //public async Task<IReadOnlyList<Employee>> GetActiveAsync(CancellationToken ct = default)
    //{
    //    return await context.Employees
    //        .AsNoTracking()
    //        .ToListAsync(ct);
    //}

    //public async Task<IReadOnlyList<Employee>> GetActiveWithTasksAsync(
    //    DateTime from,
    //    DateTime to,
    //    CancellationToken ct = default)
    //{
    //    return await context.Entry(employee)
    //        .Collection(e => e.EmployeeTasks)
    //        .Query()
    //        .Where(et => et.EndTime > from && et.StartTime < to)
    //        .LoadAsync(ct);

    //}
    public Task<IReadOnlyList<Employee>> GetActiveAsync(CancellationToken ct = default)
    {
        throw new NotImplementedException();
    }

    public Task<IReadOnlyList<Employee>> GetActiveWithTasksAsync(DateTime from, DateTime to, CancellationToken ct = default)
    {
        throw new NotImplementedException();
    }
}

