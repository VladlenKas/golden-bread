using GoldenBread.Domain.ValueObjects;

namespace GoldenBread.Domain.Interfaces.Services;

public interface IWorkCalendar
{
    // Спецификации
    public int MaxPlanningDays { get; }

    // Управление временными зонами
    public TimeZoneInfo TimeZone { get; }

    // Работа с рабочими днями
    bool IsWorkDay(DateTimeOffset date);
    bool IsWorkDay(DateTime date);
    DateTimeOffset GetNextWorkDayStart(DateTimeOffset from);
    DateTimeOffset GetPreviousWorkDayEnd(DateTimeOffset from);

    // Работа с временными интервалами
    IEnumerable<TimeSlot> GetWorkIntervals(DateTimeOffset from);
    TimeSpan GetWorkTimeBetween(DateTimeOffset from, DateTimeOffset to);
}
