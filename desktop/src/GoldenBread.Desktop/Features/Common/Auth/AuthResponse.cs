using GoldenBread.Desktop.Features.Common.Account;

namespace GoldenBread.Desktop.Features.Common.Auth;

public sealed record AuthResponse(
    int Id,
    string? Session,
    UserRole? Role,
    VerificationStatus VerificationStatus,
    string? UserInfo = null,
    string? SessionInfo = null);