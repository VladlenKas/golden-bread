namespace GoldenBread.Application.Features.CompanyProfile.Commands.ChangeEmail;

public sealed record ChangeEmailCommand(
    string NewEmail,
    string Password) : IRequest<Unit>;
