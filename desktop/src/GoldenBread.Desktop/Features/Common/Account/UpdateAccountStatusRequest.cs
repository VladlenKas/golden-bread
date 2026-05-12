namespace GoldenBread.Desktop.Features.Common.Account;

public record UpdateAccountStatusRequest(int AccountId, VerificationStatus Status);