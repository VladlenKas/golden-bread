namespace GoldenBread.Application.Behaviors.Validation.Rules;

public static class EmailRule
{
    public static IRuleBuilderOptions<T, string> BeValidEmail<T>(
        this IRuleBuilder<T, string> rule)
    {
        return rule
            .NotEmpty()
            .EmailAddress()
            .MinimumLength(2)
            .MaximumLength(100);
    }
}
