using GoldenBread.Application.Abstractions.Behaviors.Validation.Rules;

namespace GoldenBread.Application.Features.CompanyProfile.Commands.ChangeEmail;

public sealed class ChangeEmailCommandValidator : AbstractValidator<ChangeEmailCommand>
{
    public ChangeEmailCommandValidator()
    {
        RuleFor(x => x.NewEmail)
            .BeValidEmail();
    }
}

