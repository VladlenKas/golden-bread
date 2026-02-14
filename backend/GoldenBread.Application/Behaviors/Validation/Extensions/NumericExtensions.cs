namespace GoldenBread.Application.Behaviors.Validation.Extensions;

public static class NumericExtensions
{
    public static IRuleBuilderOptions<T, string> OnlyDigits<T>(
        this IRuleBuilder<T, string> ruleBuilder)
    {
        return ruleBuilder
            .Must(value => value.All(char.IsDigit))
            .WithMessage("Поле может содержать только цифры");
    }
}
