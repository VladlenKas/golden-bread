using GoldenBread.Application.Abstractions.Behaviors.Validation.Extensions;

namespace GoldenBread.Application.Abstractions.Behaviors.Validation.Rules;

public static class CredentialsDataRules
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

    public static IRuleBuilderOptions<T, string> BeValidPassword<T>(
        this IRuleBuilder<T, string> rule)
    {
        return rule
            .NotEmpty()
            .MinimumLength(8);
    }

    public static IRuleBuilderOptions<T, string> BeValidName<T>(
        this IRuleBuilder<T, string> rule)
    {
        return rule
            .NotEmpty()
            .MinimumLength(2)
            .MaximumLength(100);
    }

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

    public static IRuleBuilderOptions<T, string> BeValidOgrn<T>(
        this IRuleBuilder<T, string> rule)
    {
        return rule
            .NotEmpty()
            .Length(13)
            .OnlyDigits()
            .Must(x => x[0] == '1' || x[0] == '5')
                .WithMessage("ОГРН должен начинаться с 1 (основной) или 5 (ГРН)")
            .Must(x => uint.Parse(x.Substring(1, 2)) >= 2)
                .WithMessage("Год регистрации должен быть не ранее 2002")
            .Must(x => uint.Parse(x.Substring(4, 2)) is >= 1 and <= 99)
                .WithMessage("Код субъекта РФ должен быть в диапазоне 01-99");
    }

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
