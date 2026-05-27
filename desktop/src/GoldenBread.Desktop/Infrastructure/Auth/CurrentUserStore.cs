using GoldenBread.Desktop.Features.Common;
using GoldenBread.Desktop.Features.Common.Account;

namespace GoldenBread.Desktop.Infrastructure.Auth;

public sealed class CurrentUserStore
{
    public string? Info { get; set; }
    public string? SessionInfo { get; set; }
    public int? UserId { get; private set; }
    public UserRole? Role { get; private set; }
    public VerificationStatus? VerificationStatus { get; private set; }
    public bool IsAuthenticated => UserId.HasValue;

    public void Authenticate(
        int id, 
        UserRole role, 
        VerificationStatus status, 
        string? UserInfo,
        string? SessionInfo)
    {
        UserId = id;
        Role = role;
        VerificationStatus = status;
        Info = UserInfo;
        this.SessionInfo = SessionInfo;
    }

    public void Logout()
    {
        UserId = null;
        Role = null;
        VerificationStatus = null;
        Info = null;
        SessionInfo = null;
    }
}