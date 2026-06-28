namespace GoldenBread.Desktop.Features.References.Products.Models;

public class SeasonFilterOption
{
    public Season Season { get; }
    public int Year { get; }
    public string DisplayText { get; }

    public SeasonFilterOption(Season season, int year)
    {
        Season = season;
        Year = year;
        DisplayText = $"{LocalizedSeason(season)} {year}";
    }

    public static string LocalizedSeason(Season season) => season switch
    {
        Season.Winter => "Зима",
        Season.Spring => "Весна",
        Season.Summer => "Лето",
        Season.Autumn => "Осень",
        _ => "Неизвестно"
    };

}

public enum Season
{
    Winter,
    Spring,
    Summer,
    Autumn
}
