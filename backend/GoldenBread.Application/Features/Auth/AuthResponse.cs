using GoldenBread.Domain.Enums;

namespace GoldenBread.Application.Features.Auth;

public sealed record class AuthResponse(
    int Id,
    string Session,
    DateTime SessionExpiresAt,
    VerificationStatus VerificationStatus);