using GoldenBread.Domain.Interfaces.Services;

namespace GoldenBread.Domain.Services;

public class WorkScheduleCalculator : IWorkScheduleCalculator
{
    private const int WorkStartHour = 9;
    private const int LunchStartHour = 13;
    private const int LunchEndHour = 14;
    private const int WorkEndHour = 18;
    private const int MorningWorkMinutes = (LunchStartHour - WorkStartHour) * 60; // 240
    private const int AfternoonWorkMinutes = (WorkEndHour - LunchEndHour) * 60;   // 240
    private const int TotalWorkDayMinutes = MorningWorkMinutes + AfternoonWorkMinutes; // 480

    public DateTime GetWorkStart(DateTime date) =>
        new DateTime(date.Year, date.Month, date.Day, WorkStartHour, 0, 0, DateTimeKind.Utc);

    public DateTime GetWorkEnd(DateTime date) =>
        new DateTime(date.Year, date.Month, date.Day, WorkEndHour, 0, 0, DateTimeKind.Utc);

    public bool IsWorkDay(DateTime date) => date.DayOfWeek != DayOfWeek.Saturday && date.DayOfWeek != DayOfWeek.Sunday;

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

    public DateTime AddWorkMinutes(DateTime start, int minutes)
    {
        if (minutes <= 0) return start;

        var current = start;
        int remainingMinutes = minutes;

        while (remainingMinutes > 0)
        {
            // Если не рабочий день, перейти на следующий рабочий
            if (!IsWorkDay(current))
            {
                current = GetWorkStart(current.AddDays(1));
                continue;
            }

            var workStart = GetWorkStart(current);
            var workEnd = GetWorkEnd(current);
            var lunchStart = new DateTime(current.Year, current.Month, current.Day, LunchStartHour, 0, 0, DateTimeKind.Utc);
            var lunchEnd = new DateTime(current.Year, current.Month, current.Day, LunchEndHour, 0, 0, DateTimeKind.Utc);

            // Если текущее время до начала рабочего дня
            if (current < workStart)
                current = workStart;

            // Если текущее время в обед
            if (current >= lunchStart && current < lunchEnd)
                current = lunchEnd;

            // Если текущее время после рабочего дня
            if (current >= workEnd)
            {
                current = GetWorkStart(current.AddDays(1));
                continue;
            }

            // Считаем доступные минуты до конца рабочего дня (с учетом обеда)
            int availableMinutes;
            if (current < lunchStart)
            {
                // Утро: до обеда или до конца дня если успеваем
                var minutesToLunch = (int)(lunchStart - current).TotalMinutes;
                var minutesAfterLunch = (int)(workEnd - lunchEnd).TotalMinutes;

                if (remainingMinutes <= minutesToLunch)
                {
                    return current.AddMinutes(remainingMinutes);
                }

                availableMinutes = minutesToLunch + minutesAfterLunch;
            }
            else
            {
                // После обеда
                availableMinutes = (int)(workEnd - current).TotalMinutes;
            }

            if (remainingMinutes <= availableMinutes)
            {
                // Все помещается в текущий день
                if (current < lunchStart && current.AddMinutes(remainingMinutes) > lunchStart)
                {
                    // Переходим через обед
                    var minutesBeforeLunch = (int)(lunchStart - current).TotalMinutes;
                    var minutesAfterLunch = remainingMinutes - minutesBeforeLunch;
                    return lunchEnd.AddMinutes(minutesAfterLunch);
                }
                return current.AddMinutes(remainingMinutes);
            }

            // Не помещается, переходим на следующий день
            remainingMinutes -= availableMinutes;
            current = GetWorkStart(current.AddDays(1));
        }

        return current;
    }

    public int CalculateWorkMinutes(DateTime start, DateTime end)
    {
        if (end <= start) return 0;

        int totalMinutes = 0;
        var current = start;

        while (current < end)
        {
            if (!IsWorkDay(current))
            {
                current = GetWorkStart(current.AddDays(1));
                continue;
            }

            var workStart = GetWorkStart(current);
            var workEnd = GetWorkEnd(current);
            var lunchStart = new DateTime(current.Year, current.Month, current.Day, LunchStartHour, 0, 0, DateTimeKind.Utc);
            var lunchEnd = new DateTime(current.Year, current.Month, current.Day, LunchEndHour, 0, 0, DateTimeKind.Utc);

            var dayStart = current > workStart ? current : workStart;
            if (dayStart >= workEnd)
            {
                current = GetWorkStart(current.AddDays(1));
                continue;
            }

            // Корректируем начало если в обед
            if (dayStart >= lunchStart && dayStart < lunchEnd)
                dayStart = lunchEnd;

            var dayEnd = end < workEnd ? end : workEnd;
            if (dayEnd <= dayStart)
            {
                current = GetWorkStart(current.AddDays(1));
                continue;
            }

            // Вычитаем обед если диапазон его пересекает
            if (dayStart < lunchStart && dayEnd > lunchStart)
            {
                totalMinutes += (int)(lunchStart - dayStart).TotalMinutes;
                if (dayEnd > lunchEnd)
                    totalMinutes += (int)(dayEnd - lunchEnd).TotalMinutes;
            }
            else
            {
                totalMinutes += (int)(dayEnd - dayStart).TotalMinutes;
            }

            current = GetWorkStart(current.AddDays(1));
        }

        return totalMinutes;
    }
}

