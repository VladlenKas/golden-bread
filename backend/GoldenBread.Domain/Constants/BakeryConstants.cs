namespace GoldenBread.Domain.Constants;

public static class BakeryConstants
{
    /// <summary>
    /// На сколько дней перенести дату поставки заказа, 
    /// чтобы успеть оформить недостающие ингредиенты 
    /// </summary>
    public static readonly TimeSpan ReservationTimeout = TimeSpan.FromDays(3);

    // Рабочий график Golden Bread
    public const int WorkStartHour = 8;
    public const int LunchStartHour = 12;
    public const int LunchEndHour = 13;
    public const int WorkEndHour = 17;
    public const int MorningWorkMinutes = (LunchStartHour - WorkStartHour) * 60; // 240
    public const int AfternoonWorkMinutes = (WorkEndHour - LunchEndHour) * 60;   // 240
    public const int TotalWorkDayMinutes = MorningWorkMinutes + AfternoonWorkMinutes; // 480
}
