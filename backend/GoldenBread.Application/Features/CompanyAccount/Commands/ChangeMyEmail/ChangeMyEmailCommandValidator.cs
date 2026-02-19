using GoldenBread.Application.Behaviors.Validation.Rules;

namespace GoldenBread.Application.Features.CompanyAccount.Commands.ChangeMyEmail;

public sealed class ChangeMyEmailCommandValidator : AbstractValidator<ChangeMyEmailCommand>
{
    public ChangeMyEmailCommandValidator()
    {
        RuleFor(x => x.NewEmail)
            .BeValidEmail();
    }
}

