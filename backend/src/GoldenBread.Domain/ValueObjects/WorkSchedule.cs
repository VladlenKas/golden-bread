namespace GoldenBread.Domain.ValueObjects;

public class WorkSchedule
{
    public int WorkStartHour { get; private set; }
    public int WorkEndHour { get; private set; }
    public int LunchStartHour { get; private set; }
    public int LunchEndHour { get; private set; }
    public int MaxPlanningDays { get; private set; }

    public static WorkSchedule Create(
        int workStartHour,
        int workEndHour,
        int lunchStartHour,
        int lunchEndHour,
        int maxPlanningDays)
    {
        return new WorkSchedule
        {
            WorkStartHour = workStartHour,
            WorkEndHour = workEndHour,
            LunchStartHour = lunchStartHour,
            LunchEndHour = lunchEndHour,
            MaxPlanningDays = maxPlanningDays,
        };
    }

    public static WorkSchedule Default()
    {
        return Create(
            WorkScheduleConstants.WorkStartHour,
            WorkScheduleConstants.WorkEndHour,
            WorkScheduleConstants.LunchStartHour,
            WorkScheduleConstants.LunchEndHour,
            WorkScheduleConstants.MaxPlanningDays);
    }

    public TimeSlot[] GetWorkIntervalsForDay(DateTime date, TimeSpan offset)
    {
        var day = date.Date;

        var morningStart = new DateTimeOffset(day.AddHours(WorkStartHour), offset);
        var morningEnd = new DateTimeOffset(day.AddHours(LunchStartHour), offset);

        var afternoonStart = new DateTimeOffset(day.AddHours(LunchEndHour), offset);
        var afternoonEnd = new DateTimeOffset(day.AddHours(WorkEndHour), offset);

        return
        [
            new TimeSlot(morningStart, morningEnd),
            new TimeSlot(afternoonStart, afternoonEnd)
        ];
    }
}

internal static class WorkScheduleConstants
{
    public const int WorkStartHour = 8;
    public const int LunchStartHour = 12;
    public const int LunchEndHour = 13;
    public const int WorkEndHour = 17;
    public const int MaxPlanningDays = 90;
}
