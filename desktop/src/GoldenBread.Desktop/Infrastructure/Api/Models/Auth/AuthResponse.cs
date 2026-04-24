using GoldenBread.Desktop.Infrastructure.Common;

namespace GoldenBread.Desktop.Infrastructure.Api.Models.Auth;

public sealed record AuthResponse(
    int Id,
    string? Session,
    UserRole? Role,
    VerificationStatus VerificationStatus);