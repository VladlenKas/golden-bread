namespace GoldenBread.Desktop.Infrastructure.Api.Models.Auth;

public sealed record AuthRequest(
    string Email,
    string Password,
    int PortalType = 0);
