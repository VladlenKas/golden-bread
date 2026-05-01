namespace GoldenBread.Desktop.Features.Common.Models;

public sealed record AuthRequest(
    string Email,
    string Password,
    int PortalType = 0);
