namespace GoldenBread.Application.Abstractions.Behaviors.Validation.Extensions;

public static class NumericExtensions
{
    public static IRuleBuilderOptions<T, string> OnlyDigits<T>(
        this IRuleBuilder<T, string> rule)
    {
        return rule
            .Must(value => value.All(char.IsDigit))
            .WithMessage("Поле может содержать только цифры");
    }
}
