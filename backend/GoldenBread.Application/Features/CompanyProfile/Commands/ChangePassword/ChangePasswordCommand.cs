namespace GoldenBread.Application.Features.CompanyProfile.Commands.ChangePassword;

public sealed record class ChangePasswordCommand(
    string OldPassword,
    string NewPassword) : IRequest<Unit>;