using GoldenBread.Application.Abstractions.Data;
using GoldenBread.Application.Abstractions.Data.Repositories;
using GoldenBread.Domain.Entities;

namespace GoldenBread.Infrastructure.Data.Repositories;

public class EmployeeRepository(IGoldenBreadContext context) : IEmployeeRepository
{
    public async Task<IReadOnlyList<Employee>> GetAllAsync(CancellationToken ct = default)
    {
        return await context.Employees
            .Include(e => e.EmployeeTasks)
            .AsNoTracking()
            .ToListAsync(ct);
    }

    public async Task<Employee?> GetByIdAsync(int id, CancellationToken ct = default)
    {
        return await context.Employees
            .FirstOrDefaultAsync(e => e.EmployeeId == id, ct);
    }

    public async Task AddAsync(Employee employee, CancellationToken ct = default)
    {
        await context.Employees.AddAsync(employee, ct);
    }
}

