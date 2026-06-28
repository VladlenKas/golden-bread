using GoldenBread.Domain.Enums;

namespace GoldenBread.Domain.Extensions;

public static class SeasonExtensions
{
    public static Season GetSeason(this DateOnly date)
    {
        return date.Month switch
        {
            3 or 4 or 5 => Season.Spring,
            6 or 7 or 8 => Season.Summer,
            9 or 10 or 11 => Season.Autumn,
            _ => Season.Winter
        };
    }

    public static string ToRussianString(this Season season)
    {
        return season switch
        {
            Season.Winter => "Зима",
            Season.Spring => "Весна",
            Season.Summer => "Лето",
            Season.Autumn => "Осень",
            _ => throw new ArgumentOutOfRangeException(nameof(season), season, null)
        };
    }
}