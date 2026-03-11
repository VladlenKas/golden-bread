using GoldenBread.Domain.Constants;
using GoldenBread.Domain.Entities;
using GoldenBread.Domain.Interfaces.Services;

namespace GoldenBread.Domain.Services;

public class WorkScheduleCalculator : IWorkScheduleCalculator
{
    public DateTime GetWorkStart(DateTime date) =>
        new DateTime(date.Year, date.Month, date.Day, BakeryConstants.WorkStartHour, 0, 0, DateTimeKind.Utc);

    public DateTime GetWorkEnd(DateTime date) =>
        new DateTime(date.Year, date.Month, date.Day, BakeryConstants.WorkEndHour, 0, 0, DateTimeKind.Utc);

    public bool IsWorkDay(DateTime date) =>
        date.DayOfWeek is not DayOfWeek.Saturday and not DayOfWeek.Sunday;

    public DateTime AddWorkDays(DateTime start, int workDays)
    {
        var current = start.Date;
        int addedDays = 0;

        while (addedDays < workDays)
        {
            current = current.AddDays(1);
            if (IsWorkDay(current))
                addedDays++;
        }

        return GetWorkStart(current);
    }

    /// <summary>
    /// Добавляет рабочие минуты к дате, учитывая график 8-17 с обедом
    /// </summary>
    public DateTime AddWorkMinutes(DateTime start, int minutes)
    {
        if (minutes <= 0) return start;

        var current = start;
        var remaining = minutes;

        while (remaining > 0)
        {
            // Пропускаем выходные
            if (!IsWorkDay(current))
            {
                current = GetWorkStart(current.AddDays(1));
                continue;
            }

            var dayStart = GetWorkStart(current);
            var dayEnd = GetWorkEnd(current);
            var lunchStart = new DateTime(current.Year, current.Month, current.Day, BakeryConstants.LunchStartHour, 0, 0, DateTimeKind.Utc);
            var lunchEnd = new DateTime(current.Year, current.Month, current.Day, BakeryConstants.LunchEndHour, 0, 0, DateTimeKind.Utc);

            // Если до начала рабочего дня
            if (current < dayStart)
                current = dayStart;

            // Если в обед — перепрыгиваем
            if (current >= lunchStart && current < lunchEnd)
                current = lunchEnd;

            // Если после работы — на следующий день
            if (current >= dayEnd)
            {
                current = GetWorkStart(current.AddDays(1));
                continue;
            }

            // Сколько минут доступно с текущего момента до конца дня?
            int availableToday;
            if (current < lunchStart)
            {
                // Утро: до обеда + после обеда
                var toLunch = (int)(lunchStart - current).TotalMinutes;
                var afterLunch = BakeryConstants.AfternoonWorkMinutes;
                availableToday = toLunch + afterLunch;
            }
            else
            {
                // После обеда
                availableToday = (int)(dayEnd - current).TotalMinutes;
            }

            if (remaining <= availableToday)
            {
                // Влезаем в сегодняшний день
                if (current < lunchStart)
                {
                    var toLunch = (int)(lunchStart - current).TotalMinutes;
                    if (remaining <= toLunch)
                        return current.AddMinutes(remaining); // Успеваем до обеда

                    // Перепрыгиваем обед
                    var afterLunch = remaining - toLunch;
                    return lunchEnd.AddMinutes(afterLunch);
                }
                return current.AddMinutes(remaining);
            }

            // Не влезаем — переносим на следующий день
            remaining -= availableToday;
            current = GetWorkStart(current.AddDays(1));
        }

        return current;
    }

    /// <summary>
    /// Когда освободится сотрудник, учитывая его текущие задачи и рабочий график
    /// </summary>
    public DateTime GetNextAvailableTime(Employee employee, DateTime from)
    {
        var lastTask = employee.EmployeeTasks
            .Where(t => t.EndTime.HasValue && t.EndTime.Value > from)
            .OrderByDescending(t => t.EndTime)
            .FirstOrDefault();

        var candidate = lastTask?.EndTime ?? from;

        // Если попали на выходные/вечер — сдвигаем на начало следующего рабочего дня
        return SnapToWorkTime(candidate);
    }

    /// <summary>
    /// "Прилипает" к рабочему времени: если вне работы — возвращает начало следующего рабочего дня
    /// </summary>
    public DateTime SnapToWorkTime(DateTime date)
    {
        if (!IsWorkDay(date))
        {
            var nextDay = date.AddDays(1);
            while (!IsWorkDay(nextDay))
                nextDay = nextDay.AddDays(1);
            return GetWorkStart(nextDay);
        }

        var workStart = GetWorkStart(date);
        var workEnd = GetWorkEnd(date);
        var lunchStart = new DateTime(date.Year, date.Month, date.Day, BakeryConstants.LunchStartHour, 0, 0, DateTimeKind.Utc);
        var lunchEnd = new DateTime(date.Year, date.Month, date.Day, BakeryConstants.LunchEndHour, 0, 0, DateTimeKind.Utc);

        if (date < workStart) return workStart;
        if (date >= lunchStart && date < lunchEnd) return lunchEnd;
        if (date >= workEnd) return GetWorkStart(date.AddDays(1));

        return date;
    }
}

