namespace GoldenBread.Desktop.Infrastructure.Constants;

public static class ConstantRegularExpressions
{
    // For Attributes
    public const string Name = @"^[А-Яа-яЁё]+(?:[ -][А-Яа-яЁё]+)*$";
    public const string Details = @"^[А-Яа-яЁё.,\- ]+$";
    public const string NotRequiredName = @"^(?=.{2,35}$)[ЁёА-Яа-я]+(?:[ -][ЁёА-Яа-я]+)*$";
    public const string SupplierName = @"^(?:""[a-zA-Zа-яА-ЯёЁ]+""|[a-zA-Zа-яА-ЯёЁ]+)(?:[ -](?:""[a-zA-Zа-яА-ЯёЁ]+""|[a-zA-Zа-яА-ЯёЁ]+))*$";
    public const string Email = @"^$|^[^@\s]+@[^@\s]+\.[^@\s]+$";
    public const string Phone = @"^$|^_ ___ ___ __ __$|^8 \d{3} \d{3} \d{2} \d{2}$";
    public const string Ogrn = @"^[15] (0[2-9]|[1-9]\d)(0[1-9]|[1-9]\d) \d{4} \d{4}$";
    public const string Inn = @"^(?!0000 0000 00$)\d{4} \d{4} \d{2}$";
    public const string HexColor = @"^#[0-9A-Fa-f]{6}$";
}
