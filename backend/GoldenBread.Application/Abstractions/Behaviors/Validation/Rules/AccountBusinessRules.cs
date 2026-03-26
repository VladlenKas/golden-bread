using GoldenBread.Application.Abstractions.Data.Repositories;

namespace GoldenBread.Application.Abstractions.Behaviors.Validation.Rules;

public static class AccountBusinessRules
{
    public static IRuleBuilderOptions<T, string> MustBeUniqueEmail<T>(
        this IRuleBuilder<T, string> rule,
        IAccountRepository repo,
        int? excludeId = null) where T : class
    {
        return rule.MustAsync(async (email, ct) =>
            !await repo.ExistsByEmailAsync(email, excludeId, ct))
            .WithMessage("Email уже занят")
            .WithErrorCode("Email");
    }
}
