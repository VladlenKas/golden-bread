namespace GoldenBread.Desktop.Infrastructure.Helpers;

public static class InputFormatter
{
    private const string EmptyPhoneMask = "_ ___ ___ __ __";

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
}
