using FluentValidation;
using GoldenBread.Application.Common.Abstractions.Repositories;
using GoldenBread.Application.Common.Abstractions.Services;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static System.Net.Mime.MediaTypeNames;

namespace GoldenBread.Application.Features.Auth.Commands.RegisterCompany;

public class RegisterCompanyCommandValidator : AbstractValidator<RegisterCompanyCommand>
{
    private readonly IAccountRepository _accountRepository;
    private readonly ICompanyRepository _companyRepository;

    public RegisterCompanyCommandValidator( 
        IAccountRepository accountRepository,
        ICompanyRepository companyRepository,
        INumericValidator validator)
    {
        _accountRepository = accountRepository;
        _companyRepository = companyRepository;

        RuleFor(x => x.Email)
            .NotEmpty()
            .EmailAddress()
            .MustAsync(BeUniqueEmail)
                .WithMessage("Пользователь с такой почтой уже существует");

        RuleFor(x => x.Name)
            .NotEmpty()
            .MustAsync(BeUniqueCompanyName)
                .WithMessage("Компания с таким названием уже существует");

        RuleFor(x => x.Inn)
            .NotEmpty()
            .Length(12)
            .MustAsync(validator.IsNumeric)
                .WithMessage("ИНН должен содержать только цифры")
            .MustAsync(BeUniqueInn)
                .WithMessage("Компания с таким ИНН уже зарегистрирована");

        RuleFor(x => x.Ogrn)
            .NotEmpty()
            .Length(13)
            .MustAsync(validator.IsNumeric)
                .WithMessage("ОГРН должен содержать только цифры")
            .MustAsync(BeUniqueOgrn)
                .WithMessage("Компания с таким ОГРН уже зарегистрирована");

        RuleFor(x => x.Password)
            .NotEmpty()
            .MinimumLength(8);
    }

    private async Task<bool> BeUniqueEmail(
        string email,
        CancellationToken cancellationToken)
    {
        return await _accountRepository.GetByEmailAsync(email, cancellationToken) is null;
    }

    private async Task<bool> BeUniqueCompanyName(
        string name,
        CancellationToken cancellationToken)
    {
        return await _companyRepository.ExistsAsync(c => c.Name == name, cancellationToken);
    }

    private async Task<bool> BeUniqueInn(
        string inn,
        CancellationToken cancellationToken)
    {
        return await _companyRepository.ExistsAsync(c => c.Inn == inn, cancellationToken);
    }

    private async Task<bool> BeUniqueOgrn(
        string ogrn,
        CancellationToken cancellationToken)
    {
        return await _companyRepository.ExistsAsync(c => c.Ogrn == ogrn, cancellationToken);
    }
}
