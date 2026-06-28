using GoldenBread.Application.Abstractions.Data;
using GoldenBread.Application.Features.EmployeeTasks.Dtos;
using Microsoft.EntityFrameworkCore;

namespace GoldenBread.Application.Features.EmployeeTasks.Queries;

public sealed record GetEmployeeTaskByIdQuery(int Id) : IRequest<EmployeeTaskDetailResponse?>;

public sealed class GetEmployeeTaskByIdQueryHandler(
    IGoldenBreadContext context)
    : IRequestHandler<GetEmployeeTaskByIdQuery, EmployeeTaskDetailResponse?>
{
    public async Task<EmployeeTaskDetailResponse?> Handle(
        GetEmployeeTaskByIdQuery query,
        CancellationToken ct)
    {
        var task = await context.EmployeeTasks
            .AsNoTracking()
            .Include(t => t.Employee)
            .Include(t => t.OrderItem)
                .ThenInclude(oi => oi.Order)
                    .ThenInclude(o => o.Company)
            .Include(t => t.OrderItem)
                .ThenInclude(oi => oi.Batch)
                    .ThenInclude(b => b.Product)
            .FirstOrDefaultAsync(t => t.EmployeeTaskId == query.Id, ct);

        if (task is null) return null;

        return new EmployeeTaskDetailResponse(
            task.EmployeeTaskId,
            task.Employee.Fullname,
            task.OrderItem.Batch.Product.Name,
            task.OrderItem.OrderId,
            task.OrderItem.Order.Company?.Name ?? "-",
            task.AssignedQuantity,
            task.CompletedQuantity,
            task.StartTime,
            task.EndTime,
            task.Status,
            task.OrderItem.TotalAmount,
            $"{task.OrderItem.UnitsPerBatch} ед. в партии");
    }
}