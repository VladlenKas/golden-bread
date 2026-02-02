using GoldenBread.Contracts.Requests;
using GoldenBread.Contracts.Responses;
using Refit;
using System.Threading.Tasks;

namespace GoldenBread.Desktop.Api;

public interface IUserApi
{
    [Post("/Auth/login/user")]
    Task<IApiResponse<LoginUserResponse>> Login([Body] LoginRequest request);
}
