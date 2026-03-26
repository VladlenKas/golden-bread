using GoldenBread.Application.Abstractions.Data.Repositories;

namespace GoldenBread.Application.Abstractions.Behaviors.Validation.Rules;

public static class CompanyBusinessRules
{
    public static IRuleBuilderOptions<T, string> MustBeUniqueName<T>(
        this IRuleBuilder<T, string> rule,
        ICompanyRepository repo,
        int? excludeId = null) where T : class
    {
        return rule.MustAsync(async (name, ct) =>
            !await repo.ExistsByNameAsync(name, excludeId, ct))
            .WithMessage("Название уже занято")
            .WithErrorCode("Name");
    }

    public static IRuleBuilderOptions<T, string> MustBeUniqueInn<T>(
        this IRuleBuilder<T, string> rule,
        ICompanyRepository repo,
        int? excludeId = null) where T : class
    {
        return rule.MustAsync(async (inn, ct) =>
            !await repo.ExistsByInnAsync(inn, excludeId, ct))
            .WithMessage("ИНН уже занят")
            .WithErrorCode("Inn");
    }

    public static IRuleBuilderOptions<T, string> MustBeUniqueOgrn<T>(
        this IRuleBuilder<T, string> rule,
        ICompanyRepository repo,
        int? excludeId = null) where T : class
    {
        return rule.MustAsync(async (ogrn, ct) =>
            !await repo.ExistsByOgrnAsync(ogrn, excludeId, ct))
            .WithMessage("ОГРН уже занят")
            .WithErrorCode("Ogrn");
    }

    public static IRuleBuilderOptions<T, string> MustBeUniquePhone<T>(
        this IRuleBuilder<T, string> rule,
        ICompanyRepository repo,
        int? excludeId = null) where T : class
    {
        return rule.MustAsync(async (phone, ct) =>
            !await repo.ExistsByPhoneAsync(phone, excludeId, ct))
            .WithMessage("Номер телефона уже занят")
            .WithErrorCode("Phone");
    }

    public static IRuleBuilderOptions<T, string> MustBeUniqueAddress<T>(
        this IRuleBuilder<T, string> rule,
        ICompanyRepository repo,
        int? excludeId = null) where T : class
    {
        return rule.MustAsync(async (address, ct) =>
            !await repo.ExistsByAddressAsync(address, excludeId, ct))
            .WithMessage("Адрес уже занят")
            .WithErrorCode("Address");
    }
}
