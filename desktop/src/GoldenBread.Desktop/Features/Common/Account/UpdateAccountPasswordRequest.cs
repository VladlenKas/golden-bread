namespace GoldenBread.Desktop.Features.Common.Account;

public record UpdateAccountPasswordRequest(int AccountId, string NewPassword);