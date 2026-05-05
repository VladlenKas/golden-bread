namespace GoldenBread.Application.Features.Account.Commands.UpdateAccountEmail;

public sealed record UpdateAccountEmailCommand(int AccountId, string NewEmail) : IRequest<bool>;