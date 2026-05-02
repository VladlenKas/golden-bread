using DynamicData;
using GoldenBread.Desktop.Features.Common.Models;

namespace GoldenBread.Desktop.Infrastructure.Auth;

public sealed class CurrentUserStore
{
    public int? UserId { get; private set; }
    public UserRole Role { get; private set; } = UserRole.None;
    public VerificationStatus? VerificationStatus { get; private set; }
    public bool IsAuthenticated => UserId.HasValue;

    public void Authenticate(int id, UserRole role, VerificationStatus status)
    {
        UserId = id;
        Role = role;
        VerificationStatus = status;
    }

    public void Logout()
    {
        UserId = null;
        Role = UserRole.None;
        VerificationStatus = null;
    }
}