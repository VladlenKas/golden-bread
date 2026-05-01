namespace GoldenBread.Desktop.Features.Common.Models;

public sealed record AuthResponse(
    int Id,
    string? Session,
    UserRole? Role,
    VerificationStatus VerificationStatus);