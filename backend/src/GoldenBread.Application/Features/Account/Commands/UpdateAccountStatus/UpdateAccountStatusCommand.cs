using GoldenBread.Domain.Enums;

namespace GoldenBread.Application.Features.Account.Commands.UpdateAccountStatus;

public sealed record UpdateAccountStatusCommand(int AccountId, VerificationStatus Status) : IRequest<bool>;