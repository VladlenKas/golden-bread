using GoldenBread.Domain.Enums;

namespace GoldenBread.Application.Features.Auth;

public sealed record class AuthResponse(
    int Id,
    AccountType AccountType,
    VerificationStatus VerificationStatus);