using GoldenBread.Application.Abstractions.Data;
using GoldenBread.Application.Abstractions.Repositories;
using GoldenBread.Domain.Entities;

namespace GoldenBread.Infrastructure.Repositories;

public class EmployeeTaskRepository(IGoldenBreadContext context) : IEmployeeTaskRepository
{
    public async Task BulkCreateAsync(IEnumerable<EmployeeTask> tasks, CancellationToken cancellationToken = default)
    {
        context.EmployeeTasks.AddRange(tasks);
        await context.SaveChangesAsync(cancellationToken);
    }

    public async Task DeleteByOrderIdAsync(int orderId, CancellationToken cancellationToken = default)
    {
        var tasks = await context.EmployeeTasks
            .Where(et => context.OrderItems
                .Where(oi => oi.OrderId == orderId)
                .Select(oi => oi.OrderItemId)
                .Contains(et.OrderItemId))
            .ToListAsync(cancellationToken);

        context.EmployeeTasks.RemoveRange(tasks);
        await context.SaveChangesAsync(cancellationToken);
    }
}

