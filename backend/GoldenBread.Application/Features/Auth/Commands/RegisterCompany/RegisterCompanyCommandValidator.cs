using GoldenBread.Application.Common.Abstractions.Data;
using GoldenBread.Application.Common.Validation.Extensions;

namespace GoldenBread.Application.Features.Auth.Commands.RegisterCompany;

public class RegisterCompanyCommandValidator : AbstractValidator<RegisterCompanyCommand>
{
    private readonly IGoldenBreadContext _context;

    public RegisterCompanyCommandValidator(IGoldenBreadContext context)
    {
        _context = context;

        RuleFor(x => x.Email)
            .Cascade(CascadeMode.Stop)
            .NotEmpty()
            .EmailAddress()
            .MustAsync(BeUniqueEmail)
                .WithMessage("Компания с такой электронной почтой уже существует");

        RuleFor(x => x.Name)
            .Cascade(CascadeMode.Stop)
            .NotEmpty()
            .MinimumLength(2)
            .MustAsync(BeUniqueName)
                .WithMessage("Компания с таким названием уже существует");

        RuleFor(x => x.Inn)
            .Cascade(CascadeMode.Stop)
            .NotEmpty()
            .Length(10)
            .OnlyDigits()
            .Must(x => x != "0000000000")
                .WithMessage("ИНН не может состоять из нулей")
            .MustAsync(BeUniqueInn)
                .WithMessage("Компания с таким ИНН уже существует");

        RuleFor(x => x.Ogrn)
            .Cascade(CascadeMode.Stop)
            .NotEmpty()
            .Length(13)
            .OnlyDigits()
            .Must(x => x[0] == '1' || x[0] == '5')  
                .WithMessage("ОГРН должен начинаться с 1 (основной) или 5 (ГРН)")
            .Must(x => uint.Parse(x.Substring(1, 2)) >= 2)  
                .WithMessage("Год регистрации должен быть не ранее 2002")
            .Must(x => uint.Parse(x.Substring(4, 2)) is >= 1 and <= 99)  
                .WithMessage("Код субъекта РФ должен быть в диапазоне 01-99")
            .MustAsync(BeUniqueOgrn)
                .WithMessage("Компания с таким ОГРН уже существует");

        RuleFor(x => x.Password)
            .Cascade(CascadeMode.Stop)
            .NotEmpty()
            .MinimumLength(8);
    }

    private async Task<bool> BeUniqueEmail(string email, CancellationToken cancellationToken) =>
        !await _context.Accounts.AnyAsync(a => a.Email == email, cancellationToken);

    private async Task<bool> BeUniqueName(string name, CancellationToken cancellationToken) =>
        !await _context.Companies.AnyAsync(c => c.Name == name, cancellationToken);

    private async Task<bool> BeUniqueInn(string inn, CancellationToken cancellationToken) =>
        !await _context.Companies.AnyAsync(c => c.Inn == inn, cancellationToken);

    private async Task<bool> BeUniqueOgrn(string ogrn,CancellationToken cancellationToken) =>
        !await _context.Companies.AnyAsync(c => c.Ogrn == ogrn, cancellationToken);
}
