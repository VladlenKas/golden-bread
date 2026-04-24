using GoldenBread.Domain.Entities;
using GoldenBread.Domain.Interfaces.Services;
using GoldenBread.Domain.ValueObjects;

namespace GoldenBread.Domain.Extensions;

public static class EmployeeExtensions
{
    /// <summary>
    /// Возвращает все рабочие слоты сотрудника за день
    /// (сейчас использует IWorkCalendar. Передавать инициализрованный объект класса!
    /// В будущем вместо него будет график выбранного сотрудника)
    /// </summary>
    /// <returns>Временные отрезки дня типа TimeSlot</returns>
    public static List<TimeSlot> GetTimeSlots(
        this Employee employee,
        DateTimeOffset date,
        IWorkCalendar calendar)
    {
        var localDate = TimeZoneInfo.ConvertTime(date, calendar.TimeZone);
        var workShiftBlocks = calendar.GetWorkIntervals(localDate);

        // Получаем задачи только для этого дня
        var tasks = employee.EmployeeTasks
            .Where(t => t.StartTime.HasValue && t.EndTime.HasValue)
            .Select(t => new
            {
                Start = TimeZoneInfo.ConvertTime(t.StartTime!.Value, calendar.TimeZone),
                End = TimeZoneInfo.ConvertTime(t.EndTime!.Value, calendar.TimeZone)
            })
            .Where(t => t.Start.Date == localDate.Date)
            .OrderBy(t => t.Start)
            .ToList();

        var allSlots = new List<TimeSlot>();

        foreach (var workBlock in workShiftBlocks)
        {
            var blockStart = workBlock.Start;
            var blockEnd = workBlock.End;

            // Находим задачи, пересекающие текущий блок
            var blockTasks = tasks
                .Where(t => t.End > blockStart && t.Start < blockEnd)
                .ToList();

            if (blockTasks.Count == 0)
            {
                // Весь блок свободен
                allSlots.Add(TimeSlot.Free(blockStart, blockEnd));
                continue;
            }

            var currentTime = blockStart;

            foreach (var task in blockTasks)
            {
                // Обрезаем задачу по границам блока
                var taskStart = task.Start < blockStart ? blockStart : task.Start;
                var taskEnd = task.End > blockEnd ? blockEnd : task.End;

                // Свободное время перед задачей
                if (taskStart > currentTime)
                    allSlots.Add(TimeSlot.Free(currentTime, taskStart));

                // Занятое время (обрезанное!)
                allSlots.Add(TimeSlot.Busy(taskStart, taskEnd));

                currentTime = taskEnd;
            }

            // Остаток блока
            if (currentTime < blockEnd)
                allSlots.Add(TimeSlot.Free(currentTime, blockEnd));
        }

        return allSlots.OrderBy(s => s.Start).ToList();
    }

    /// <summary>
    /// Суммирует общее время задач за период и возвращает в минутах
    /// </summary>
    /// <returns>Общее количество рабочих минут за диапозон</returns>
    public static double GetCurrentLoadMinutes(
        this Employee employee,
        DateTimeOffset from,
        DateTimeOffset deadline)
    {
        if (deadline <= from)
            return 0;

        return employee.EmployeeTasks
            .Where(t => t.StartTime.HasValue && t.EndTime.HasValue)
            .Select(t =>
            {
                var start = t.StartTime!.Value;
                var end = t.EndTime!.Value;

                var overlapStart = start > from ? start : from;
                var overlapEnd = end < deadline ? end : deadline;

                return overlapStart < overlapEnd
                    ? (overlapEnd - overlapStart).TotalMinutes
                    : 0;
            })
            .Sum();
    }

    public static double GetFreeCapacityMinutes(
        this Employee employee,
        DateTimeOffset from,
        DateTimeOffset deadline,
        IWorkCalendar calendar)
    {
        var totalWorkMinutes = calendar.GetWorkTimeBetween(from, deadline).TotalMinutes;
        var currentLoadMinutes = employee.GetCurrentLoadMinutes(from, deadline);

        var freeMinutes = totalWorkMinutes - currentLoadMinutes;
        return freeMinutes > 0 ? freeMinutes : 0;
    }

    public static double GetLoadPercentage(
        this Employee employee,
        DateTimeOffset from,
        DateTimeOffset deadline,
        IWorkCalendar calendar)
    {
        var totalWorkMinutes = calendar.GetWorkTimeBetween(from, deadline).TotalMinutes;

        if (totalWorkMinutes <= 0)
            return 0;

        var currentLoadMinutes = employee.GetCurrentLoadMinutes(from, deadline);

        var percentage = (currentLoadMinutes / totalWorkMinutes) * 100;
        return percentage > 100 ? 100 : percentage;
    }
}
