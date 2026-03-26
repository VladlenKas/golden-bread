using Refit;
using System.Threading.Tasks;

namespace GoldenBread.Desktop.Api;

public interface IUserApi
{
    [Post("/Auth/login/user")]
    Task<IApiResponse<LoginUserResponse>> Login([Body] LoginRequest request);
}

public sealed record LoginRequest(
    string Email,
    string Password,
    int PortalType = 0);

public sealed record LoginUserResponse(
    int Id,
    VerificationStatus Status);

public enum VerificationStatus
{
    Pending,
    Approved,
    Rejected,
    Suspended,
}
