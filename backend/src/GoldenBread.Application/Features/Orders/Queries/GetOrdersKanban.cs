using GoldenBread.Application.Abstractions.Data;
using GoldenBread.Application.Features.Orders.Dtos;
using GoldenBread.Domain.Enums;

namespace GoldenBread.Application.Features.Orders.Queries;

public sealed record GetOrdersKanbanQuery : IRequest<List<OrderKanbanItem>>;

public sealed class GetOrdersKanbanQueryHandler(
    IGoldenBreadContext context,
    IUnitOfWork unitOfWork)
    : IRequestHandler<GetOrdersKanbanQuery, List<OrderKanbanItem>>
{
    public async Task<List<OrderKanbanItem>> Handle(
        GetOrdersKanbanQuery query,
        CancellationToken ct)
    {
        var orders = await context.Orders
            .AsTracking()
            .Include(o => o.OrderItems)
                .ThenInclude(oi => oi.EmployeeTasks)
            .Include(o => o.Company)
            .ToListAsync(ct);

        var updated = false;

        foreach (var order in orders)
        {
            // InProgress → Completed если все задачи сотрудников выполнены
            if (order.Status == OrderStatus.InProgress
                && order.OrderItems
                    .SelectMany(oi => oi.EmployeeTasks)
                    .All(t => t.Status == Domain.Enums.TaskStatus.Completed))
            {
                order.UpdateStatus(OrderStatus.Completed);
                updated = true;
            }
        }

        if (updated)
            await unitOfWork.SaveChangesAsync(ct);

        return orders.Select(o => new OrderKanbanItem(
            o.OrderId,
            o.Company?.Name ?? "-",
            o.StartDate,
            o.EndDate,
            o.CreatedAt,
            o.OrderItems.Sum(i => i.TotalAmount),
            o.OrderItems.Count,
            o.OrderItems.SelectMany(oi => oi.EmployeeTasks).Count(),
            o.OrderItems.SelectMany(oi => oi.EmployeeTasks).Count(t => t.Status == Domain.Enums.TaskStatus.Completed),
            o.Status
        )).ToList();
    }
}