namespace GoldenBread.Desktop.Infrastructure.Constants;

public static class ConstantRegularExpressions
{
    // For Attributes
    public const string Name = @"^[А-Яа-яЁё]+(?:[ -][А-Яа-яЁё]+)*$";
    public const string NotRequiredName = @"^(?=.{2,35}$)[ЁёА-Яа-я]+(?:[ -][ЁёА-Яа-я]+)*$";
}
