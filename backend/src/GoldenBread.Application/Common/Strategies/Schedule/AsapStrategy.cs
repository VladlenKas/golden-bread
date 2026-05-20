using GoldenBread.Domain.Entities;
using GoldenBread.Domain.Extensions;
using GoldenBread.Domain.Interfaces.Services;
using GoldenBread.Domain.Interfaces.Strategies;
using GoldenBread.Domain.ValueObjects;

namespace GoldenBread.Application.Common.Strategies.Schedule;

public class AsapStrategy : ISchedulingStrategy
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
        var startSearch = DateTimeOffset.UtcNow.AddDays(1);
        var currentDate = startSearch.Date;
        var endDate = deadline.Date;

        while (currentDate <= endDate && remainingUnits > 0)
        {
            var dayOffset = new DateTimeOffset(currentDate, calendar.TimeZone.BaseUtcOffset);
            var slots = employee.GetTimeSlots(dayOffset, calendar);

            foreach (var freeSlot in slots.Where(s => s.Type == SlotType.Free))
            {
                if (remainingUnits <= 0)
                    break;

                var slotMinutes = freeSlot.Duration.TotalMinutes;
                var maxUnitsInSlot = (int)(slotMinutes / unitTimeMinutes);

                // В этот остаток слота даже 1 единица не влезает — пропускаем
                if (maxUnitsInSlot <= 0)
                    continue;

                var takeUnits = Math.Min(remainingUnits, maxUnitsInSlot);
                var takeMinutes = takeUnits * unitTimeMinutes;
                var duration = TimeSpan.FromMinutes(takeMinutes);

                var startTime = freeSlot.Start;
                var endTime = startTime.Add(duration);

                // Если выходим за дедлайн — откатываем всё назначение
                if (endTime > deadline)
                    return new List<EmployeeTask>();

                var task = EmployeeTask.Create(
                    employeeId: employee.EmployeeId,
                    orderItemId: orderItem.OrderItemId,
                    startTime: startTime,
                    endTime: endTime,
                    assignedQuantity: takeUnits,
                    completedQuantity: 0);

                result.Add(task);
                remainingUnits -= takeUnits;
            }

            currentDate = currentDate.AddDays(1);
        }

        // Если остались нераспределённые единицы — значит места не хватило
        return remainingUnits == 0 ? result : new List<EmployeeTask>();
    }

    public bool IsBetter(ScheduleResult current, ScheduleResult best)
    {
        if (!best.IsFeasible) return current.IsFeasible;
        if (!current.IsFeasible) return false;
        return current.PlanEnd < best.PlanEnd;
    }
}