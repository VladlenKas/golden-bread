using FluentValidation;
using GoldenBread.Application.Repositories;
using GoldenBread.Contracts.Requests;
namespace GoldenBread.Application.Validators;

public class CompanyValidator : AbstractValidator<RegisterCompanyRequest>
{
    public CompanyValidator(
        IAccountRepository accountRepository,
        ICompanyRepository companyRepository)
    {
        RuleFor(x => x.Email)
            .NotEmpty()
                .WithMessage("Электронная почта обязательна для заполнения")
            .EmailAddress()
                .WithMessage("Введите корректный адрес электронной почты")
            .MustAsync(async (command, email, ct) =>
                await accountRepository.GetByEmailAsync(email) == null)
                .WithMessage("Пользователь с такой почтой уже существует");

        RuleFor(x => x.Name)
            .NotEmpty()
                .WithMessage("Название компании обязательно для заполнения")
            .MustAsync(async (command, name, ct) =>
                await companyRepository.GetByNameAsync(name) == null)
                .WithMessage("Компания с таким названием уже существует");

        RuleFor(x => x.Inn)
            .NotEmpty()
                .WithMessage("ИНН обязателен для заполнения")
            .Length(10, 12)
                .WithMessage("ИНН должен содержать 10 или 12 цифр")
            .Matches(@"^\d+$")
                .WithMessage("ИНН должен содержать только цифры")
            .MustAsync(async (command, inn, ct) =>
                await companyRepository.GetByInnAsync(inn) == null)
                .WithMessage("Компания с таким ИНН уже зарегистрирована");

        RuleFor(x => x.Ogrn)
            .NotEmpty()
                .WithMessage("ОГРН обязателен для заполнения")
            .Length(13, 15)
                .WithMessage("ОГРН должен содержать 13 или 15 цифр")
            .Matches(@"^\d+$")
                .WithMessage("ОГРН должен содержать только цифры")
            .MustAsync(async (command, ogrn, ct) =>
                await companyRepository.GetByOgrnAsync(ogrn) == null)
                .WithMessage("Компания с таким ОГРН уже зарегистрирована");


        //RuleFor(x => x.Phone)
        //    .MustAsync(async (command, phone, ct) =>
        //        await companyRepository.GetByPhoneAsync(phone!) == null)
        //    .WithMessage("Phone already exists")
        //    .When(x => !string.IsNullOrWhiteSpace(x.Phone));

        //RuleFor(x => x.Address)
        //    .MustAsync(async (command, address, ct) =>
        //        await companyRepository.GetByAddressAsync(address!) == null)
        //    .WithMessage("Address already exists")
        //    .When(x => !string.IsNullOrWhiteSpace(x.Address));

        RuleFor(x => x.Password)
            .NotEmpty()
            .MinimumLength(8)
            .WithMessage("Длина пароля должна быть не менее 8 символов");
    }
}
