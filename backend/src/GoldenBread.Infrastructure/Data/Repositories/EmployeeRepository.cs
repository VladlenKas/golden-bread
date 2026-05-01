using GoldenBread.Application.Abstractions.Data;
using GoldenBread.Application.Abstractions.Data.Repositories;
using GoldenBread.Domain.Entities;

namespace GoldenBread.Infrastructure.Data.Repositories;

public class EmployeeRepository(IGoldenBreadContext context) : IEmployeeRepository
{
    public async Task<IReadOnlyList<Employee>> GetAllAsync(CancellationToken ct = default)
    {
        return await context.Employees
            .AsNoTracking()
            .ToListAsync(ct);
    }

    public async Task<Employee?> GetByIdAsync(int id, CancellationToken ct = default)
    {
        return await context.Employees
            .AsNoTracking()
            .FirstOrDefaultAsync(e => e.EmployeeId == id, ct);
    }
}

