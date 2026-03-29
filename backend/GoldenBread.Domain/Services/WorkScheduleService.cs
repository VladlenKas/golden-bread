using GoldenBread.Domain.Constants;
using GoldenBread.Domain.Entities;
using GoldenBread.Domain.Extensions;
using GoldenBread.Domain.Interfaces.Services;

namespace GoldenBread.Domain.Services;

public class WorkScheduleService : IBakeryScheduleService
{
    private record WorkDayInfo(bool IsWorkDay, DateTime Start, DateTime End);

    public bool IsWorkDay(DateTime date) => GetWorkDayInfo(date).IsWorkDay;
    public DateTime GetWorkStart(DateTime date) => GetWorkDayInfo(date).Start;
    public DateTime GetWorkEnd(DateTime date) => GetWorkDayInfo(date).End;

    /// <summary>
    /// Считает все рабочие дни, 
    /// исключая выходные и текущий день
    /// </summary>
    /// <param name="start"></param>
    /// <param name="workDays"></param>
    /// <returns></returns>
    public DateTime AddWorkDays(DateTime start, int workDays)
    {
        var current = start.ToUtc().Date;

        while (workDays > 0)
        {
            current = current.AddDays(1);
            if (IsWorkDay(current))
                workDays--;
        }

        return GetWorkStart(current);
    }

    /// <summary>
    /// Добавляет рабочие минуты к дате, учитывая график с обедом
    /// </summary>
    public DateTime AddWorkMinutes(DateTime start, int minutes)
    {
        if (minutes <= 0) return start;

        var current = SnapToWorkTime(start);
        var remaining = minutes;

        int iterations = WorkScheduleConstants.MaxPlanningDays;
        while (remaining > 0 && iterations-- > 0)
        {
            var dayEnd = GetWorkEnd(current);
            var (lunchStart, lunchEnd) = GetLunchBreak(current);

            int availableBeforeLunch = current < lunchStart
                ? (int)(lunchStart - current).TotalMinutes
                : 0;

            int availableAfterLunch = current < lunchEnd
                ? (int)(dayEnd - lunchEnd).TotalMinutes
                : (int)(dayEnd - current).TotalMinutes;

            int availableToday = availableBeforeLunch + availableAfterLunch;

            if (availableToday <= 0)
            {
                current = SnapToWorkTime(current.AddDays(1));
                continue;
            }

            if (remaining <= availableBeforeLunch)
                return current.AddMinutes(remaining);

            if (remaining <= availableToday)
            {
                // Если уже после обеда — просто добавляем к current
                if (current >= lunchEnd)
                {
                    return current.AddMinutes(remaining);
                }

                // Иначе — через обед
                var afterLunchNeeded = remaining - availableBeforeLunch;
                return lunchEnd.AddMinutes(afterLunchNeeded);
            }

            // Переносим на следующий день
            remaining -= availableToday;
            current = SnapToWorkTime(current.AddDays(1));
        }

        throw new InvalidOperationException
            ($"Cannot add {minutes} work minutes: exceeds max planning period of {WorkScheduleConstants.MaxPlanningDays} days");
    }

    /// <summary>
    /// Когда освободится сотрудник, учитывая его текущие задачи и рабочий график.
    /// EmployeeTasks должны быть уже загружены (Include). 
    /// </summary>
    public DateTime GetNextAvailableTime(Employee employee, DateTime from)
    {
        var sortedTasks = employee.EmployeeTasks
            .Where(t => 
                t.EndTime.HasValue && 
                t.StartTime.HasValue && 
                t.EndTime.Value > from)
            .OrderBy(t => t.StartTime!.Value)
            .ToList();

        var candidate = SnapToWorkTime(from);

        foreach (var task in sortedTasks)
        {
            // Если задача начинается после candidate — окно найдено
            if (task.StartTime!.Value > candidate)
                return candidate;

            // Задача пересекается с candidate — двигаем candidate
            if (task.EndTime!.Value > candidate)
                candidate = SnapToWorkTime(task.EndTime.Value);
        }

        return candidate;
    }

    /// <summary>
    /// "Прилипает" к рабочему времени: если вне работы — 
    /// возвращает начало следующего рабочего дня
    /// </summary>
    public DateTime SnapToWorkTime(DateTime date)
    {
        var current = date.ToUtc();
        int iterations = WorkScheduleConstants.MaxPlanningDays;

        while (iterations-- > 0)
        {
            if (!IsWorkDay(current))
            {
                current = current.AddDays(1);
                continue;
            }

            var workStart = GetWorkStart(current);
            var workEnd = GetWorkEnd(current);
            var (lunchStart, lunchEnd) = GetLunchBreak(current);

            if (current < workStart) return workStart;
            if (current >= lunchStart && current < lunchEnd) return lunchEnd;
            if (current >= workEnd)
            {
                current = GetWorkStart(current.AddDays(1));
                continue; 
            }

            return current;
        }

        throw new InvalidOperationException("Cannot find work day within planning period");
    }

    private DateTime GetWorkDayPart(DateTime date, int hour)
    {
        var utcDate = date.ToUtc();

        return new DateTime(utcDate.Year, utcDate.Month, utcDate.Day, hour, 0, 0, DateTimeKind.Utc);
    }

    private WorkDayInfo GetWorkDayInfo(DateTime date)
    {
        var utcDate = date.ToUtc();

        return new WorkDayInfo(
            utcDate.DayOfWeek is not (DayOfWeek.Saturday or DayOfWeek.Sunday),
            GetWorkDayPart(date, WorkScheduleConstants.WorkStartHour),
            GetWorkDayPart(date, WorkScheduleConstants.WorkEndHour)
        );
    }

    private (DateTime Start, DateTime End) GetLunchBreak(DateTime date)
    {
        var utcDate = date.ToUtc();
        var datePart = new DateTime(utcDate.Year, utcDate.Month, utcDate.Day, 0, 0, 0, DateTimeKind.Utc);
        
        return 
        (
            datePart.AddHours(WorkScheduleConstants.LunchStartHour),
            datePart.AddHours(WorkScheduleConstants.LunchEndHour)
        );
    }
}

