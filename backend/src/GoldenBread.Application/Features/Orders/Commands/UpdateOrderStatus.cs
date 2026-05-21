using GoldenBread.Application.Abstractions.Data;
using GoldenBread.Application.Abstractions.Services;
using GoldenBread.Application.Common.Services;
using GoldenBread.Application.Common.Strategies.Schedule;
using GoldenBread.Application.Features.Orders.Dtos;
using GoldenBread.Application.Features.Orders.Exceptions;
using GoldenBread.Domain.Entities;
using GoldenBread.Domain.Enums;
using GoldenBread.Domain.Interfaces.Services;

namespace GoldenBread.Application.Features.Orders.Commands;

public sealed record UpdateOrderStatusCommand(UpdateOrderStatusRequest Request) : IRequest<bool>;

public sealed class UpdateOrderStatusCommandHandler(
    IGoldenBreadContext context,
    IWorkCalendar calendar,
    IUnitOfWork unitOfWork,
    IUnitConversionService unitConversion,
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
                    HandleCreatedToCanceled(order, req.CancelReason);
                    break;

                case OrderStatus.Canceled when order.Status == OrderStatus.InProgress:
                    await HandleInProgressToCanceledAsync(order, req.CancelReason, ct);
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
        var shortages = new List<IngredientShortageItem> ();

        // --- 1. Загружаем доступные партии ингредиентов ---
        var ingredientIds = order.OrderItems
            .SelectMany(oi => oi.Batch.Product.Recipes)
            .Select(r => r.IngredientId)
            .Distinct()
            .ToList();

        var allBatches = await context.IngredientBatches
            .AsTracking()
            .Include(ib => ib.SupplierIngredient)
                .ThenInclude(si => si.Ingredient)
            .Where(ib => ingredientIds.Contains(ib.SupplierIngredient.IngredientId))
            .Where(ib => ib.ExpiryDate >= order.EndDate
                      && ib.RemainingQuantity > 0
                      && ib.Status != IngredientBatchStatus.Archived)
            .OrderBy(ib => ib.DeliveryDate)
            .ToListAsync(ct);

        // --- 2. Списание ингредиентов и резервирование ---
        foreach (var item in order.OrderItems)
        {
            foreach (var recipe in item.Batch.Product.Recipes)
            {
                var requiredBase = recipe.Quantity * item.TotalUnits;
                var batches = allBatches
                    .Where(b => b.SupplierIngredient.IngredientId == recipe.IngredientId)
                    .ToList();

                var availableBase = batches.Sum(b =>
                    unitConversion.ToBaseUnit(b.RemainingQuantity, b.SupplierIngredient.Unit));

                if (availableBase < requiredBase)
                {
                    shortages.Add(new IngredientShortageItem(
                        recipe.Ingredient.Name,
                        requiredBase,
                        availableBase,
                        recipe.Ingredient.BaseUnit));
                    continue;
                }

                var remaining = requiredBase;
                foreach (var batch in batches)
                {
                    if (remaining <= 0) break;

                    var batchBase = unitConversion.ToBaseUnit(
                        batch.RemainingQuantity, batch.SupplierIngredient.Unit);

                    var toWriteOffBase = Math.Min(remaining, batchBase);
                    var toWriteOffSupplier = unitConversion.FromBaseUnit(
                        toWriteOffBase, batch.SupplierIngredient.Unit);

                    var actual = batch.TryWriteOff(toWriteOffSupplier);
                    var actualBase = unitConversion.ToBaseUnit(
                        actual, batch.SupplierIngredient.Unit);

                    remaining -= actualBase;

                    var reservation = new OrderItemIngredientReservation
                    {
                        OrderItemId = item.OrderItemId,
                        IngredientBatchId = batch.IngredientBatchId,
                        ReservedQuantity = actual,
                    };
                    context.OrderItemIngredientReservations.Add(reservation);
                }
            }
        }

        if (shortages.Count != 0)
            throw new InsufficientIngredientsException(shortages);

        // --- 3. JIT-планирование ---
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
            task.UpdateStatus(OrderStatus.Created);

        if (!scheduleResult.IsFeasible)
            throw new InvalidOperationException("Невозможно распределить задачи для заказа. Недостаточно свободных сотрудников");

        order.StartDate = DateOnly.FromDateTime(scheduleResult.PlanStart);

        // Задачи уже созданы внутри Distribute — просто сохраняем их
        context.EmployeeTasks.AddRange(scheduleResult.Tasks!);

        // --- 4. Переводим заказ и позиции в Awaiting ---
        order.UpdateStatus(OrderStatus.InProgress);
        foreach (var item in order.OrderItems)
            item.Status = OrderStatus.InProgress;
    }

    private async Task HandleInProgressToCanceledAsync(Order order, string? reason, CancellationToken ct)
    {
        var tasks = await context.EmployeeTasks
            .AsTracking()
            .Where(t => t.OrderItem.OrderId == order.OrderId)
            .ToListAsync(ct);

        foreach (var task in tasks)
        {
            if (task.Status == OrderStatus.Created)
            {
                var reservations = await context.OrderItemIngredientReservations
                    .AsTracking()
                    .Include(r => r.IngredientBatch)
                    .Where(r => r.OrderItemId == task.OrderItemId)
                    .ToListAsync(ct);

                foreach (var r in reservations)
                {
                    r.IngredientBatch.Return(r.ReservedQuantity);
                    context.OrderItemIngredientReservations.Remove(r);
                }

                task.UpdateStatus(OrderStatus.Canceled);
            }
            else if (task.Status == OrderStatus.InProgress)
            {
                task.UpdateStatus(OrderStatus.Canceled);
            }
        }

        foreach (var item in order.OrderItems)
            item.Status = OrderStatus.Canceled;

        order.Cancel(reason);
    }

    private static void HandleCreatedToCanceled(Order order, string? reason)
    {
        order.Cancel(reason);
        foreach (var item in order.OrderItems)
            item.Status = OrderStatus.Canceled;
    }

    private static void HandleCreatedToCompleted(Order order)
    {
        order.Status = OrderStatus.Completed;
        foreach (var item in order.OrderItems)
            item.Status = OrderStatus.Completed;
    }

    private async Task HandleInProgressToCompletedAsync(Order order, CancellationToken ct)
    {
        var tasks = await context.EmployeeTasks
            .AsTracking()
            .Where(t => t.OrderItem.OrderId == order.OrderId)
            .ToListAsync(ct);

        foreach (var task in tasks)
            task.UpdateStatus(OrderStatus.Completed);

        foreach (var item in order.OrderItems)
            item.Status = OrderStatus.Completed;

        order.Status = OrderStatus.Completed;
    }
}