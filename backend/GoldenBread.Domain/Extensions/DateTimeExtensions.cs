namespace GoldenBread.Domain.Extensions;

public static class DateTimeExtensions
{
    public static DateTime ToUtc(this DateTime date) =>
        date.Kind == DateTimeKind.Local
            ? date.ToUniversalTime()
            : date;
}
