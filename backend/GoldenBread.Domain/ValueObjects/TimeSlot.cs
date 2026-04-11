namespace GoldenBread.Domain.ValueObjects;

public enum SlotType { None, Busy, Free };

public class TimeSlot(
    DateTimeOffset start, 
    DateTimeOffset end, 
    SlotType type = SlotType.None)
{
    public DateTimeOffset Start { get; } = start;
    public DateTimeOffset End { get; } = end;
    public SlotType Type { get; } = type;

    public TimeSpan Duration => End - Start;

    public static TimeSlot Busy(DateTimeOffset start, DateTimeOffset end)
        => new(start, end, SlotType.Busy);

    public static TimeSlot Free(DateTimeOffset start, DateTimeOffset end)
        => new(start, end, SlotType.Free);

    public static TimeSlot ForDay(DateOnly date)
    {
        var dayStart = date.ToDateTime(TimeOnly.MinValue);
        var dayEnd = date.ToDateTime(new TimeOnly(23, 59, 59, 999));

        return new TimeSlot(
            new DateTimeOffset(dayStart, TimeSpan.Zero),
            new DateTimeOffset(dayEnd, TimeSpan.Zero));
    }
}