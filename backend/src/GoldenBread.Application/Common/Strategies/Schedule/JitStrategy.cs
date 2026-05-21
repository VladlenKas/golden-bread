using GoldenBread.Domain.Entities;
using GoldenBread.Domain.Extensions;
using GoldenBread.Domain.Interfaces.Services;
using GoldenBread.Domain.Interfaces.Strategies;
using GoldenBread.Domain.ValueObjects;

namespace GoldenBread.Application.Common.Strategies.Schedule;

public class JitStrategy : ISchedulingStrategy
{
    public List<EmployeeTask> TrySchedule(
        DbEntities.Employee employee,
        OrderItem orderItem,
        int units,
        DateTimeOffset deadline,
        IWorkCalendar calendar)
    {
        var result = new List<EmployeeTask>();
        var unitTimeMinutes = orderItem.Batch.Product.ProductionTimeMinutes;

        if (unitTimeMinutes <= 0)
            throw new InvalidOperationException("ProductionTimeMinutes must be > 0");

        var remainingUnits = units;

        // JIT: от дедлайна назад к завтрашнему дню
        var endDate = deadline.Date;
        var startDate = DateTimeOffset.UtcNow.AddDays(1).Date;
        var currentDate = endDate;

        while (currentDate >= startDate && remainingUnits > 0)
        {
            var dayOffset = new DateTimeOffset(currentDate, calendar.TimeZone.BaseUtcOffset);
            var slots = employee.GetTimeSlots(dayOffset, calendar);

            // Идем по слотам с конца дня к началу
            foreach (var freeSlot in slots.Where(s => s.Type == SlotType.Free).Reverse())
            {
                if (remainingUnits <= 0)
                    break;

                // Задача должна закончиться не позже дедлайна и не позже конца слота
                var slotEndLimit = freeSlot.End < deadline ? freeSlot.End : deadline;

                // Если дедлайн раньше начала слота — слот не подходит
                if (slotEndLimit <= freeSlot.Start)
                    continue;

                var availableMinutes = (slotEndLimit - freeSlot.Start).TotalMinutes;
                var maxUnitsInSlot = (int)(availableMinutes / unitTimeMinutes);

                if (maxUnitsInSlot <= 0)
                    continue;

                var takeUnits = Math.Min(remainingUnits, maxUnitsInSlot);
                var takeMinutes = takeUnits * unitTimeMinutes;
                var duration = TimeSpan.FromMinutes(takeMinutes);

                // Прижимаем к правой границе доступного окна
                var endTime = slotEndLimit;
                var startTime = endTime.Subtract(duration);

                // Страховка: не вылезли за начало слота
                if (startTime < freeSlot.Start)
                    continue;

                var task = EmployeeTask.Create(
                    employeeId: employee.EmployeeId,
                    orderItemId: orderItem.OrderItemId,
                    startTime: startTime.UtcDateTime,
                    endTime: endTime.UtcDateTime,
                    assignedQuantity: takeUnits,
                    completedQuantity: 0);

                result.Add(task);
                remainingUnits -= takeUnits;
            }

            currentDate = currentDate.AddDays(-1);
        }

        // Размещали с конца — переворачиваем в хронологический порядок
        if (result.Count > 1)
            result.Reverse();

        return remainingUnits == 0 ? result : new List<EmployeeTask>();
    }

    public bool IsBetter(ScheduleResult current, ScheduleResult best)
    {
        // Failed всегда хуже feasible
        if (!best.IsFeasible) return current.IsFeasible;
        if (!current.IsFeasible) return false;

        // JIT лучше, когда план заканчивается позже (максимально близко к дедлайну)
        return current.PlanStart > best.PlanStart;
    }
}