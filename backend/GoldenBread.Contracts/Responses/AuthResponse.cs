namespace GoldenBread.Contracts.Responses;

public sealed record class AuthResponse(
    int Id,
    string Session,
    DateTime SessionExpiresAt,
    AccountStatus AccountStatus);