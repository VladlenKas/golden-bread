using GoldenBread.Application.Behaviors.Validation.Extensions;

namespace GoldenBread.Application.Behaviors.Validation.Rules;

public static class OgrnRule
{
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
}
