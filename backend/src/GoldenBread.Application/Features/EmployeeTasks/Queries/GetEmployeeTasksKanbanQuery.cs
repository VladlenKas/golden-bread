using GoldenBread.Application.Abstractions.Data;
using GoldenBread.Application.Features.EmployeeTasks.Dtos;
using Microsoft.EntityFrameworkCore;

namespace GoldenBread.Application.Features.EmployeeTasks.Queries;

public sealed record GetEmployeeTasksKanbanQuery : IRequest<List<EmployeeTaskKanbanItem>>;

public sealed class GetEmployeeTasksKanbanQueryHandler(
    IGoldenBreadContext context)
    : IRequestHandler<GetEmployeeTasksKanbanQuery, List<EmployeeTaskKanbanItem>>
{
    public async Task<List<EmployeeTaskKanbanItem>> Handle(
        GetEmployeeTasksKanbanQuery query,
        CancellationToken ct)
    {
        var tasks = await context.EmployeeTasks
            .AsNoTracking()
            .Include(t => t.Employee)
            .Include(t => t.OrderItem)
                .ThenInclude(oi => oi.Order)
                    .ThenInclude(o => o.Company)
            .Include(t => t.OrderItem)
                .ThenInclude(oi => oi.Batch)
                    .ThenInclude(b => b.Product)
            .ToListAsync(ct);

        return tasks.Select(t => new EmployeeTaskKanbanItem(
            t.EmployeeTaskId,
            t.Employee.Fullname,
            t.OrderItem.Batch.Product.Name,
            t.OrderItem.OrderId,
            t.OrderItem.Order.Company?.Name ?? "-",
            t.StartTime,
            t.EndTime,
            t.AssignedQuantity,
            t.CompletedQuantity,
            t.Status
        )).ToList();
    }
}