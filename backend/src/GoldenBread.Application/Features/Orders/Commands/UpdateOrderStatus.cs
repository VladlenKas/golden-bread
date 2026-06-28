using GoldenBread.Application.Abstractions.Data;
using GoldenBread.Application.Common.Services;
using GoldenBread.Application.Common.Strategies.Schedule;
using GoldenBread.Application.Features.Orders.Dtos;
using GoldenBread.Domain.Entities;
using GoldenBread.Domain.Enums;
using GoldenBread.Domain.Interfaces.Services;

namespace GoldenBread.Application.Features.Orders.Commands;

public sealed record UpdateOrderStatusCommand(UpdateOrderStatusRequest Request) : IRequest<bool>;

public sealed class UpdateOrderStatusCommandHandler(
    IGoldenBreadContext context,
    IWorkCalendar calendar,
    IUnitOfWork unitOfWork,
    ScheduleTaskDistributor scheduler)
    : IRequestHandler<UpdateOrderStatusCommand, bool>
{
    public async Task<bool> Handle(UpdateOrderStatusCommand command, CancellationToken ct)
    {
        var req = command.Request;
        var order = await context.Orders
            .AsTracking()
            .Include(o => o.OrderItems)
                .ThenInclude(oi => oi.Batch)
                    .ThenInclude(b => b.Product)
                        .ThenInclude(p => p.Recipes)
                            .ThenInclude(r => r.Ingredient)
            .Include(o => o.Company)
            .FirstOrDefaultAsync(o => o.OrderId == req.OrderId, ct);

        if (order is null)
            return false;

        await unitOfWork.BeginTransactionAsync(ct);
        try
        {
            switch (req.NewStatus)
            {
                case OrderStatus.InProgress when order.Status == OrderStatus.Created:
                    await HandleCreatedToInProgressAsync(order, ct);
                    break;

                case OrderStatus.Canceled when order.Status == OrderStatus.Created:
                    HandleCreatedToCanceled(order);
                    break;

                case OrderStatus.Canceled when order.Status == OrderStatus.InProgress:
                    await HandleInProgressToCanceledAsync(order, ct);
                    break;

                case OrderStatus.Completed when order.Status == OrderStatus.Created:
                    HandleCreatedToCompleted(order);
                    break;

                case OrderStatus.Completed when order.Status == OrderStatus.InProgress:
                    await HandleInProgressToCompletedAsync(order, ct);
                    break;

                default:
                    throw new InvalidOperationException(
                        $"Переход из {order.Status} в {req.NewStatus} запрещён");
            }

            await unitOfWork.CommitAsync(ct);
            return true;
        }
        catch
        {
            await unitOfWork.RollbackAsync(ct);
            throw;
        }
    }

    private async Task HandleCreatedToInProgressAsync(Order order, CancellationToken ct)
    {
        // --- JIT-планирование задач сотрудников ---
        var employees = await context.Employees
            .AsNoTracking()
            .Include(e => e.EmployeeTasks)
            .ToListAsync(ct);

        var deadlineDate = order.EndDate.ToDateTime(TimeOnly.MinValue);
        var deadline = new DateTimeOffset(
            deadlineDate.AddHours(17),
            calendar.TimeZone.BaseUtcOffset);

        var jit = new JitStrategy();
        var scheduleResult = scheduler.Distribute(
            order.OrderItems.ToList(),
            employees,
            jit,
            deadline);

        foreach (var task in scheduleResult.Tasks!)
            task.UpdateStatus(Domain.Enums.TaskStatus.InProgress);

        if (!scheduleResult.IsFeasible)
        {
            var now = DateTimeOffset.UtcNow;

            // Проверяем дедлайн (всегда есть)
            if (deadline < now)
                throw new InvalidOperationException("Невозможно распределить задачи: дата окончания заказа уже прошла");

            // Проверяем старт, только если назначен
            if (order.StartDate.HasValue)
            {
                var start = order.StartDate.Value.ToDateTime(TimeOnly.MinValue);
                if (start > DateTime.UtcNow.Date)
                    throw new InvalidOperationException("Невозможно распределить задачи: дата начала заказа позже текущего дня");
            }

            throw new InvalidOperationException("Невозможно распределить задачи для заказа. Недостаточно свободных сотрудников");
        }

        order.StartDate = DateOnly.FromDateTime(scheduleResult.PlanStart);

        context.EmployeeTasks.AddRange(scheduleResult.Tasks!);

        // --- Переводим заказ в InProgress ---
        order.UpdateStatus(OrderStatus.InProgress);
    }

    private async Task HandleInProgressToCanceledAsync(Order order, CancellationToken ct)
    {
        var tasks = await context.EmployeeTasks
            .AsTracking()
            .Where(t => t.OrderItem.OrderId == order.OrderId)
            .ToListAsync(ct);

        foreach (var task in tasks)
            task.UpdateStatus(Domain.Enums.TaskStatus.Canceled);

        order.Cancel();
    }

    private static void HandleCreatedToCanceled(Order order)
    {
        order.Cancel();
    }

    private static void HandleCreatedToCompleted(Order order)
    {
        order.UpdateStatus(OrderStatus.Completed);
    }

    private async Task HandleInProgressToCompletedAsync(Order order, CancellationToken ct)
    {
        var tasks = await context.EmployeeTasks
            .AsTracking()
            .Where(t => t.OrderItem.OrderId == order.OrderId)
            .ToListAsync(ct);

        if (tasks.Any(t => 
            t.Status == Domain.Enums.TaskStatus.Paused ||
            t.Status == Domain.Enums.TaskStatus.InProgress))
        {
            throw new InvalidOperationException("Для подтверждения завершения заказа все связанные задачи должны быть выполнены");
        }

        foreach (var task in tasks)
            task.UpdateStatus(Domain.Enums.TaskStatus.Completed);

        order.UpdateStatus(OrderStatus.Completed);
    }
}