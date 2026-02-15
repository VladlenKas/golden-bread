using GoldenBread.Domain.Enums;

namespace GoldenBread.Application.Features.Auth;

public sealed record class AuthResponse(VerificationStatus VerificationStatus);