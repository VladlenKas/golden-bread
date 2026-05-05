namespace GoldenBread.Desktop.Features.Common.Models;

public record UpdateAccountStatusRequest(int AccountId, VerificationStatus Status);