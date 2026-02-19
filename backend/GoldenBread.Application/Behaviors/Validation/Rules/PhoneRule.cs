namespace GoldenBread.Application.Behaviors.Validation.Rules;

public static class PhoneRule
{
    public static IRuleBuilderOptions<T, string> BeValidPhone<T>(
        this IRuleBuilder<T, string> rule)
    {
        return rule
            .Must(x => x[0] == '8')
                .WithMessage("Номер телефона должен начинаться с 8")
            .Length(11)
            .When(x => x != null);
    }
}
