namespace GoldenBread.Application.Features.CompanyAccount.Commands.ChangeMyEmail;

public sealed record class ChangeMyEmailCommand(
    string NewEmail,
    string Password) : IRequest<Unit>;
