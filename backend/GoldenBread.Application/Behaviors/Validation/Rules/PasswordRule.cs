namespace GoldenBread.Application.Behaviors.Validation.Rules;

public static class PasswordRule
{
    public static IRuleBuilderOptions<T, string> BeValidPassword<T>(
        this IRuleBuilder<T, string> rule)
    {
        return rule
            .NotEmpty()
            .MinimumLength(8);
    }
}
