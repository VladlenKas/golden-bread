namespace GoldenBread.Desktop.Infrastructure.Helpers;

public static class InputFormatter
{
    private const string EmptyPhoneMask = "_ ___ ___ __ __";
    private const string EmptyInnMask = "____ ____ __";
    private const string EmptyOgrnMask = "_ ____ ____ ____";

    //  Phone
    public static string? NormalizePhone(string? phone)
    {
        if (string.IsNullOrWhiteSpace(phone) || phone == EmptyPhoneMask)
            return null;

        return new string(phone.Where(char.IsDigit).ToArray());
    }

    public static string? FormatPhone(string? phone)
    {
        if (string.IsNullOrWhiteSpace(phone))
            return null;

        var digits = new string(phone.Where(char.IsDigit).ToArray());

        if (digits.Length != 11)
            return phone;

        return $"{digits[0]} {digits.Substring(1, 3)} {digits.Substring(4, 3)} {digits.Substring(7, 2)} {digits.Substring(9, 2)}";
    }

    // Inn
    public static string? NormalizeInn(string? inn)
    {
        if (string.IsNullOrWhiteSpace(inn) || inn == EmptyInnMask)
            return null;

        var digits = new string(inn.Where(char.IsDigit).ToArray());

        // ИНН юрлица = 10 цифр
        return digits.Length == 10 ? digits : null;
    }

    public static string? FormatInn(string? inn)
    {
        if (string.IsNullOrWhiteSpace(inn))
            return null;

        var digits = new string(inn.Where(char.IsDigit).ToArray());
        if (digits.Length != 10)
            return inn; // не форматируем если невалидный

        return $"{digits.Substring(0, 4)} {digits.Substring(4, 4)} {digits.Substring(8, 2)}";
    }

    // Ogrn
    public static string? NormalizeOgrn(string? ogrn)
    {
        if (string.IsNullOrWhiteSpace(ogrn) || ogrn == EmptyOgrnMask)
            return null;

        var digits = new string(ogrn.Where(char.IsDigit).ToArray());

        // ОГРН = 13 цифр
        return digits.Length == 13 ? digits : null;
    }

    public static string? FormatOgrn(string? ogrn)
    {
        if (string.IsNullOrWhiteSpace(ogrn))
            return null;

        var digits = new string(ogrn.Where(char.IsDigit).ToArray());
        if (digits.Length != 13)
            return ogrn; // не форматируем если невалидный

        return $"{digits[0]} {digits.Substring(1, 4)} {digits.Substring(5, 4)} {digits.Substring(9, 4)}";
    }
}
