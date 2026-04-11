using GoldenBread.Domain.Entities;
using GoldenBread.Domain.Extensions;
using GoldenBread.Domain.Interfaces.Services;
using GoldenBread.Domain.Interfaces.Strategies;
using GoldenBread.Domain.ValueObjects;

namespace GoldenBread.Application.Common.Strategies.Schedule;

public class AsapStrategy : ISchedulingStrategy
{
    public EmployeeTask? TrySchedule(
        DbEntities.Employee employee, 
        OrderItem orderItem, 
        int units,
        DateTimeOffset deadline, 
        IWorkCalendar calendar)
    {
        // Считаем сколько минут нужно
        var minutesNeeded = units * orderItem.Batch.Product.ProductionTimeMinutes;
        var duration = TimeSpan.FromMinutes(minutesNeeded);

        // Указываем точку старта (начинаем со следующего дня)
        var startSearch = DateTimeOffset.UtcNow.AddDays(1); 

        // Перебираем дни от сегодня до дедлайна
        var currentDate = startSearch.Date;
        var endDate = deadline.Date;

        while (currentDate <= endDate)
        {
            // Получаем слоты для конкретного дня (рабочее время минус занятые задачи)
            var dayOffset = new DateTimeOffset(currentDate, calendar.TimeZone.BaseUtcOffset);
            var slots = employee.GetTimeSlots(dayOffset, calendar);

            // Ищем первый свободный слот достаточной длительности
            var freeSlot = slots
                .Where(s => s.Type == SlotType.Free)
                .FirstOrDefault(s => s.Duration >= duration);

            if (freeSlot != null)
            {
                var startTime = freeSlot.Start;
                var endTime = startTime.Add(duration);

                // Проверяем, что укладываемся в дедлайн
                if (endTime <= deadline)
                {
                    // Создаем задачу сразу, без промежуточного TaskAssignment
                    return EmployeeTask.Create(
                        employeeId: employee.EmployeeId,
                        orderItemId: orderItem.OrderItemId,
                        startTime: startTime,
                        endTime: endTime,
                        assignedQuantity: units,
                        completedQuantity: 0);
                }
            }

            currentDate = currentDate.AddDays(1);
        }

        return null;
    }

    public bool IsBetter(ScheduleResult current, ScheduleResult best)
    {
        // ASAP лучше, когда заканчиваем раньше
        return current.PlanEnd < best.PlanEnd;
    }
}
