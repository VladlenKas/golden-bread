namespace GoldenBread.Desktop.Features.Common.Auth;

public sealed record AuthRequest(
    string Email,
    string Password,
    int PortalType = 0);
