namespace GoldenBread.Application.Features.Account.Commands.DeleteAccount;

public sealed record DeleteAccountCommand(int AccountId) : IRequest<bool>;