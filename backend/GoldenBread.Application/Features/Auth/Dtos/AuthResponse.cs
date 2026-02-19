using GoldenBread.Domain.Enums;

namespace GoldenBread.Application.Features.Auth.Dtos;

public sealed record class AuthResponse(
    int Id,
    VerificationStatus VerificationStatus);