using GoldenBread.Application.Abstractions.Data.Repositories;
using GoldenBread.Application.Common.Constants;

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
            .WithMessage(ValidationErrorConstants.Duplicate)
            .WithErrorCode("Name");
    }

    public static IRuleBuilderOptions<T, string> MustBeUniqueInn<T>(
        this IRuleBuilder<T, string> rule,
        ICompanyRepository repo,
        int? excludeId = null) where T : class
    {
        return rule.MustAsync(async (inn, ct) =>
            !await repo.ExistsByInnAsync(inn, excludeId, ct))
            .WithMessage(ValidationErrorConstants.Duplicate)
            .WithErrorCode("Inn");
    }

    public static IRuleBuilderOptions<T, string> MustBeUniqueOgrn<T>(
        this IRuleBuilder<T, string> rule,
        ICompanyRepository repo,
        int? excludeId = null) where T : class
    {
        return rule.MustAsync(async (ogrn, ct) =>
            !await repo.ExistsByOgrnAsync(ogrn, excludeId, ct))
            .WithMessage(ValidationErrorConstants.Duplicate)
            .WithErrorCode("Ogrn");
    }

    public static IRuleBuilderOptions<T, string> MustBeUniquePhone<T>(
        this IRuleBuilder<T, string> rule,
        ICompanyRepository repo,
        int? excludeId = null) where T : class
    {
        return rule.MustAsync(async (phone, ct) =>
            !await repo.ExistsByPhoneAsync(phone, excludeId, ct))
            .WithMessage(ValidationErrorConstants.Duplicate)
            .WithErrorCode("Phone");
    }

    public static IRuleBuilderOptions<T, string> MustBeUniqueAddress<T>(
        this IRuleBuilder<T, string> rule,
        ICompanyRepository repo,
        int? excludeId = null) where T : class
    {
        return rule.MustAsync(async (address, ct) =>
            !await repo.ExistsByAddressAsync(address, excludeId, ct))
            .WithMessage(ValidationErrorConstants.Duplicate)
            .WithErrorCode("Address");
    }
}
