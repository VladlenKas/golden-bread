using GoldenBread.Domain.Interfaces.Services;
using GoldenBread.Domain.ValueObjects;

namespace GoldenBread.Domain.Services;

public class WorkCalendar(WorkSchedule schedule, TimeZoneInfo timeZone) : IWorkCalendar
{
    public TimeZoneInfo TimeZone { get; } = timeZone;
    public int MaxPlanningDays { get; } = schedule.MaxPlanningDays;

    public bool IsWorkDay(DateTimeOffset date)
        => date.DayOfWeek is not (DayOfWeek.Saturday or DayOfWeek.Sunday);

    public bool IsWorkDay(DateTime date)
        => date.DayOfWeek is not (DayOfWeek.Saturday or DayOfWeek.Sunday);

    public DateTimeOffset GetNextWorkDayStart(DateTimeOffset from)
    {
        var localFrom = TimeZoneInfo.ConvertTime(from, TimeZone);
        var current = localFrom.Date.AddDays(1);

        while (!IsWorkDay(current))
            current = current.AddDays(1);

        var result = current.AddHours(schedule.WorkStartHour);
        return new DateTimeOffset(result, TimeZone.GetUtcOffset(result));
    }

    public DateTimeOffset GetPreviousWorkDayEnd(DateTimeOffset from)
    {
        var localFrom = TimeZoneInfo.ConvertTime(from, TimeZone);
        var current = localFrom.Date.AddDays(-1);

        while (!IsWorkDay(current))
            current = current.AddDays(-1);

        var result = current.AddHours(schedule.WorkEndHour);
        return new DateTimeOffset(result, TimeZone.GetUtcOffset(result));
    }

    public IEnumerable<TimeSlot> GetWorkIntervals(DateTimeOffset date)
    {
        var localDate = TimeZoneInfo.ConvertTime(date, TimeZone);

        if (!IsWorkDay(localDate.DateTime))
            return Array.Empty<TimeSlot>();

        return schedule.GetWorkIntervalsForDay(localDate.Date, TimeZone.GetUtcOffset(localDate.DateTime));
    }

    public TimeSpan GetWorkTimeBetween(DateTimeOffset from, DateTimeOffset to)
    {
        var localFrom = TimeZoneInfo.ConvertTime(from, TimeZone);
        var localTo = TimeZoneInfo.ConvertTime(to, TimeZone);

        if (localTo <= localFrom)
            return TimeSpan.Zero;

        var total = TimeSpan.Zero;
        var currentDate = localFrom.Date;
        var endDate = localTo.Date;

        while (currentDate <= endDate)
        {
            var dateOffset = new DateTimeOffset(currentDate, TimeZone.GetUtcOffset(currentDate));

            foreach (var interval in GetWorkIntervals(dateOffset))
            {
                var overlapStart = interval.Start > localFrom ? interval.Start : localFrom;
                var overlapEnd = interval.End < localTo ? interval.End : localTo;

                if (overlapStart < overlapEnd)
                    total += overlapEnd - overlapStart;
            }

            currentDate = currentDate.AddDays(1);
        }

        return total;
    }
}
