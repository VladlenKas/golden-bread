using GoldenBread.Application.Abstractions.Behaviors.Validation.Rules;
using GoldenBread.Application.Abstractions.Repositories;

namespace GoldenBread.Application.Features.Auth.Commands.RegisterCompany;

public sealed class RegisterCompanyCommandValidator : AbstractValidator<RegisterCompanyCommand>
{
    public RegisterCompanyCommandValidator(
        ICompanyRepository companyRepository,
        IAccountRepository accountRepository)
    {
        RuleFor(x => x.Email)
            .Cascade(CascadeMode.Stop)
            .BeValidEmail()
            .MustBeUniqueEmail(accountRepository);

        RuleFor(x => x.Name)
            .Cascade(CascadeMode.Stop)
            .BeValidName()
            .MustBeUniqueName(companyRepository);

        RuleFor(x => x.Inn)
            .Cascade(CascadeMode.Stop)
            .BeValidInn()
            .MustBeUniqueInn(companyRepository);

        RuleFor(x => x.Ogrn)
            .Cascade(CascadeMode.Stop)
            .BeValidOgrn()
            .MustBeUniqueOgrn(companyRepository);

        RuleFor(x => x.Password)
            .Cascade(CascadeMode.Stop)
            .BeValidPassword();
    }   
}
