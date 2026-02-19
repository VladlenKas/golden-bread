namespace GoldenBread.Application.Behaviors.Validation.Rules;

public static class NameRule
{
    public static IRuleBuilderOptions<T, string> BeValidName<T>(
        this IRuleBuilder<T, string> rule)
    {
        return rule
            .NotEmpty()
            .MinimumLength(2)
            .MaximumLength(100);
    }
}
