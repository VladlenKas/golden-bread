namespace GoldenBread.Application.Features.Account.Commands.UpdateAccountPassword;

public sealed record UpdateAccountPasswordCommand(int AccountId, string NewPassword) : IRequest<bool>;