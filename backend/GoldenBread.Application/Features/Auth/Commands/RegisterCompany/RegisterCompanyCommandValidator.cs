using GoldenBread.Application.Behaviors.Validation.Rules;

namespace GoldenBread.Application.Features.Auth.Commands.RegisterCompany;

public sealed class RegisterCompanyCommandValidator : AbstractValidator<RegisterCompanyCommand>
{
    public RegisterCompanyCommandValidator()
    {
        RuleFor(x => x.Email)
            .Cascade(CascadeMode.Stop)
            .BeValidEmail();

        RuleFor(x => x.Name)
            .Cascade(CascadeMode.Stop)
            .BeValidName();

        RuleFor(x => x.Inn)
            .Cascade(CascadeMode.Stop)
            .BeValidInn();

        RuleFor(x => x.Ogrn)
            .Cascade(CascadeMode.Stop)
            .BeValidOgrn();

        RuleFor(x => x.Password)
            .Cascade(CascadeMode.Stop)
            .BeValidPassword();
    }
}
