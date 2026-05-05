namespace GoldenBread.Desktop.Features.Common.Models;

public record UpdateAccountPasswordRequest(int AccountId, string NewPassword);