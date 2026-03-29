using GoldenBread.Application.Abstractions.Data;
using GoldenBread.Application.Features.CompanyCart.Services;
using GoldenBread.Domain.Constants;
using GoldenBread.Domain.Entities;
using GoldenBread.Domain.Enums;
using GoldenBread.Domain.Interfaces.Services;

namespace GoldenBread.Infrastructure.Data.Services;

public class DeliveryDateCalculator(
    IBakeryScheduleService workSchedule,
    IGoldenBreadContext context) 
{
    public DateOnly CalculateRealisticDate(
        List<CartItem> cartItems,
        OrderTariff tariff,
        DateTime now)
    {
        if (cartItems.Count == 0)
            return DateOnly.FromDateTime(workSchedule.AddWorkDays(now, 1));

        // 1. Загружаем невыполненные задачи (backlog)
        var committedMinutes = GetCommittedWorkMinutes(now);

        // 2. Добавляем наш новый заказ
        var newOrderMinutes = CalculateTotalProductionMinutes(cartItems);
        var totalQueueMinutes = committedMinutes + newOrderMinutes;

        // 3. Сколько сотрудников реально работает (без % тарифа, берем всех активных)
        var activeEmployees = context.Employees
            .Count(e => e.DeletedAt == null);

        var dailyCapacity = activeEmployees * WorkScheduleConstants.TotalWorkDayMinutes;

        // 4. Сколько дней займет вся очередь (backlog + мы)
        var totalDays = (int)Math.Ceiling((double)totalQueueMinutes / dailyCapacity);

        // 5. Считаем от сегодня (уже есть очередь, не откладываем)
        var productionEndDate = workSchedule.AddWorkDays(now, totalDays);

        // 6. Доставка
        return CalculateDeliveryDate(productionEndDate, now);
    }

    /// <summary>
    /// Считает время производства всех позиций в партии
    /// </summary>
    /// <param name="cartItems"></param>
    /// <returns></returns>
    private int CalculateTotalProductionMinutes(List<CartItem> cartItems)
    {
        return cartItems.Sum(ci =>
            ci.Quantity *
            ci.Batch.Product.ProductionTimeMinutes *
            ci.Batch.QuantityUnits);
    }

    /// <summary>
    /// Если закончили после DeliveryCutoffHour, то переносим
    /// доставку на следующий рабочий день
    /// </summary>
    /// <param name="productionEnd"></param>
    /// <param name="now"></param>
    /// <returns></returns>
    private DateOnly CalculateDeliveryDate(DateTime productionEnd, DateTime now)
    {
        if (productionEnd.Hour >= WorkScheduleConstants.DeliveryCutoffHour ||
            !workSchedule.IsWorkDay(productionEnd))
        {
            var nextDay = workSchedule.AddWorkDays(productionEnd, 1);
            return DateOnly.FromDateTime(nextDay);
        }

        return DateOnly.FromDateTime(productionEnd);
    }

    /// <summary>
    /// Суммирует оставшееся время всех невыполненных задач
    /// </summary>
    /// <param name="from"></param>
    /// <returns></returns>
    private int GetCommittedWorkMinutes(DateTime from)
    {
        var tasks = context.EmployeeTasks
            .AsNoTracking()
            .Where(t => t.Status != OrderStatus.Completed &&
                t.EndTime > from)
            .Select(t => new { t.StartTime, t.EndTime })
            .ToList();

        return tasks.Sum(t =>
            (int)(t.EndTime!.Value - t.StartTime!.Value).TotalMinutes);
    }
}
