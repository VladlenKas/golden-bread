using GoldenBread.Desktop.Infrastructure.Api.Models.Auth;
using GoldenBread.Desktop.Infrastructure.Common;

namespace GoldenBread.Desktop.Infrastructure.Auth;

public interface ICurrentUserStore
{
    int? UserId { get; }
    UserRole? Role { get; }
    VerificationStatus? VerificationStatus { get; }
    bool IsAuthenticated { get; }

    void Authenticate(int id, UserRole role, VerificationStatus status);
    void Logout();
}

public sealed class CurrentUserStore : ICurrentUserStore
{
    public int? UserId { get; private set; }
    public UserRole? Role { get; private set; }
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
        Role = null;
        VerificationStatus = null;
    }
}