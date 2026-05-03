namespace GoldenBread.Domain.Extensions;

public static class StringExtensions
{
    public static string? StringOrNullNormalize(this string? str) => 
        string.IsNullOrWhiteSpace(str) ? null : str.Trim();

    public static string? ToUpperFirstChar(this string? str) =>
        string.IsNullOrWhiteSpace(str) ? null : char.ToUpper(str[0]) + str[1..].ToLower();
}
