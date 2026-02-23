namespace GoldenBread.Application.Features.CompanyProfile.Commands.ChangeEmail;

public sealed record class ChangeEmailCommand(
    string NewEmail,
    string Password) : IRequest<Unit>;
