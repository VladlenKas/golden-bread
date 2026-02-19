using GoldenBread.Application.Behaviors.Validation.Extensions;

namespace GoldenBread.Application.Behaviors.Validation.Rules;

public static class InnRule
{
    public static IRuleBuilderOptions<T, string> BeValidInn<T>(
        this IRuleBuilder<T, string> rule)
    {
        return rule
            .NotEmpty()
            .Length(10)
            .OnlyDigits()
            .Must(x => x != "0000000000")
                .WithMessage("ИНН не может состоять из нулей");
    }
}
