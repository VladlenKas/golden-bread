using GoldenBread.Application.Abstractions.Data;
using GoldenBread.Application.Abstractions.Repositories;
using GoldenBread.Domain.Entities;

namespace GoldenBread.Infrastructure.Data.Repositories;

public class EmployeeTaskRepository(IGoldenBreadContext context) : IEmployeeTaskRepository
{
    public async Task BulkCreateAsync(IEnumerable<EmployeeTask> tasks, CancellationToken ct = default)
    {
        context.EmployeeTasks.AddRange(tasks);
    }

    public async Task DeleteByOrderIdAsync(int orderId, CancellationToken ct = default)
    {
        var tasks = await context.EmployeeTasks
            .Where(et => context.OrderItems
                .Where(oi => oi.OrderId == orderId)
                .Select(oi => oi.OrderItemId)
                .Contains(et.OrderItemId))
            .ToListAsync(ct);

        context.EmployeeTasks.RemoveRange(tasks);
    }
}

