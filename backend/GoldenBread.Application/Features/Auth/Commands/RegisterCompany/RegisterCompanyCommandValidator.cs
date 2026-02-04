using GoldenBread.Application.Common.Abstractions.Repositories;
using GoldenBread.Application.Common.Validation.Rules;

namespace GoldenBread.Application.Features.Auth.Commands.RegisterCompany;

public class RegisterCompanyCommandValidator : AbstractValidator<RegisterCompanyCommand>
{
    private readonly IAccountRepository _accountRepository;
    private readonly ICompanyRepository _companyRepository;

    public RegisterCompanyCommandValidator( 
        IAccountRepository accountRepository,
        ICompanyRepository companyRepository)
    {
        _accountRepository = accountRepository;
        _companyRepository = companyRepository;

        RuleFor(x => x.Email)
            .Cascade(CascadeMode.Stop)
            .NotEmpty()
            .EmailAddress()
            .MustAsync(BeUniqueEmail)
                .WithMessage("Почта уже занята");

        RuleFor(x => x.Name)
            .Cascade(CascadeMode.Stop)
            .NotEmpty()
            .MustAsync(BeUniqueName)
                .WithMessage("Название уже занято");

        RuleFor(x => x.Inn)
            .Cascade(CascadeMode.Stop)
            .NotEmpty()
            .Length(12)
            .MustAsync(NumericRule.IsNumeric)
                .WithMessage("ИНН должен содержать только цифры")
            .MustAsync(BeUniqueInn)
                .WithMessage("ИНН уже занят");

        RuleFor(x => x.Ogrn)
            .Cascade(CascadeMode.Stop)
            .NotEmpty()
            .Length(13)
            .MustAsync(NumericRule.IsNumeric)
                .WithMessage("ОГРН должен содержать только цифры")
            .MustAsync(BeUniqueOgrn)
                .WithMessage("ОГРН уже занят");

        RuleFor(x => x.Password)
            .Cascade(CascadeMode.Stop)
            .NotEmpty()
            .MinimumLength(8);
    }

    private async Task<bool> BeUniqueEmail(string email, CancellationToken cancellationToken) =>
        await _accountRepository.GetByEmailAsync(email, cancellationToken) is null;

    private async Task<bool> BeUniqueName(string name, CancellationToken cancellationToken) =>
        await _companyRepository.ExistsAsync(c => c.Name == name, cancellationToken);

    private async Task<bool> BeUniqueInn(string inn, CancellationToken cancellationToken) =>
        await _companyRepository.ExistsAsync(c => c.Inn == inn, cancellationToken);

    private async Task<bool> BeUniqueOgrn(string ogrn,CancellationToken cancellationToken) =>
        await _companyRepository.ExistsAsync(c => c.Ogrn == ogrn, cancellationToken);
}
