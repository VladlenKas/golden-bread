namespace GoldenBread.Application.Features.CompanyAccount.Commands.ChangeMyPassword;

public sealed record class ChangeMyPasswordCommand(
    string OldPassword,
    string NewPassword) : IRequest<Unit>;