using GoldenBread.Application.Abstractions.Data.Repositories;
using GoldenBread.Application.Abstractions.Services;
using GoldenBread.Application.Features.CompanyOrder.Dtos;
using GoldenBread.Application.Features.CompanyOrder.Services;
using GoldenBread.Domain.Entities;
using GoldenBread.Domain.Enums;
using GoldenBread.Domain.Interfaces.Services;

namespace GoldenBread.Application.Features.CompanyOrder.Commands.CreateOrder;

public class CreateOrderCommandHandler(
    ICurrentAccountContext accountContext,
    IOrderRepository orderRepository,
    IOrderItemRepository orderItemRepository,
    ICartRepository cartRepository,
    IEmployeeRepository employeeRepository,
    IEmployeeTaskRepository employeeTaskRepository,
    IOrderTariffRepository tariffRepository,
    IProductionCalculator productionCalculator,
    IIngredientReservationService ingredientService,
    IEmployeeTaskDistributor taskDistributor,
    IWorkScheduleCalculator workScheduleCalculator) : 
    IRequestHandler<CreateOrderCommand, CreateOrderResult>
{
    public async Task<CreateOrderResult> Handle(
        CreateOrderCommand command, 
        CancellationToken ct)
    {
        // 1. Получить корзину
        int companyId = await accountContext.GetRequiredCompanyIdAsync(ct);
        var cartItems = await cartRepository.GetByCompanyIdAsync(companyId, ct);

        // 2. Создать OrderItems для проверки ингредиентов
        var orderItems = cartItems.Select(ci => OrderItem
            .Create(0, 0, ci.BatchId, ci.Quantity, ci.Batch.QuantityPerBatch, ci.Batch.UnitPrice))
            .ToList();

        // 3. Проверить ингредиенты (без резервирования)
        var check = await ingredientService.CheckAsync(orderItems, ct);

        if (!check)
        {
            return new CreateOrderResult
            {
                Success = false,
                InsufficientIngredients = true,
                ProposedDeferredDate = command.DesiredDeliveryDate.AddDays(3)
            };
        }

        // 2. Получить тариф
        var tariff = await tariffRepository.GetByIdAsync(command.TariffId, ct)
            ?? throw new InvalidOperationException("Tariff not found");

        // 3. Получить активных сотрудников с задачами на период
        var now = DateTime.UtcNow;
        var activeEmployees = await employeeRepository.GetActiveWithTasksAsync(
            now, now.AddDays(30), ct);

        // 4. Рассчитать план производства
        var plan = productionCalculator.CalculatePlan(
            cartItems, tariff, now, command.DesiredDeliveryDate, activeEmployees);

        // 7. Создать заказ
        var order = Order.Create(
            companyId,
            command.TariffId,
            OrderStatus.Awaiting,
            plan.ConfirmedDeliveryDate);

        await orderRepository.CreateAsync(order, ct);

        // 8. Привязать OrderItems к заказу и сохранить
        orderItems = cartItems.Select(ci => OrderItem.Create(
            0, order.OrderId, ci.BatchId, ci.Quantity, ci.Batch.QuantityPerBatch, ci.Batch.UnitPrice))
            .ToList();

        await orderItemRepository.CreateRangeAsync(orderItems, ct);

        // 9. Зарезервировать и подтвердить ингредиенты
        await ingredientService.ReserveForOrderAsync(
            orderItems, order.OrderId, ct);

        // 10. Подготовить занятость сотрудников
        var employeeAvailableFrom = activeEmployees.ToDictionary(
            e => e.EmployeeId,
            e => CalculateNextAvailableTime(e, plan.ProductionStart));

        // 11. Распределить задачи между сотрудниками
        var allAssignments = new List<EmployeeTaskAssignment>();

        foreach (var orderItem in orderItems)
        {
            var assignments = taskDistributor.Distribute(
                orderItem,
                plan.SelectedEmployees,
                employeeAvailableFrom,
                tariff.FreeEmployeesPercent);

            allAssignments.AddRange(assignments);
        }

        // 12. Создать EmployeeTasks
        var tasks = allAssignments.Select(a => EmployeeTask.Create(
            a.EmployeeId,
            a.OrderItemId,
            a.StartTime,
            a.EndTime,
            a.AssignedQuantity,
            0)).ToList();

        await employeeTaskRepository.BulkCreateAsync(tasks, ct);

        // 13. Очистить корзину
        await cartRepository.ClearAsync(companyId, ct);

        return new CreateOrderResult
        {
            Success = true,
            OrderId = order.OrderId,
            DeliveryDate = plan.ConfirmedDeliveryDate,
            IsDeferred = false
        };
    }

    private DateTime CalculateNextAvailableTime(Employee employee, DateTime defaultStart)
    {
        var lastTask = employee.EmployeeTasks
            .Where(t => t.EndTime.HasValue)
            .OrderByDescending(t => t.EndTime)
            .FirstOrDefault();

        if (lastTask == null) return defaultStart;

        // Если последняя задача закончилась, проверяем рабочий день
        var endTime = lastTask.EndTime!.Value;
        return endTime > defaultStart ? endTime : defaultStart;
    }
}
