using GoldenBread.Desktop.Features.Common.Models;
using Refit;

namespace GoldenBread.Desktop.Infrastructure.Api;

public interface IAccountApi
{
    [Put("/api/accounts/password")]
    Task<IApiResponse> UpdatePassword([Body] UpdateAccountPasswordRequest request);

    [Put("/api/accounts/email")]
    Task<IApiResponse> UpdateEmail([Body] UpdateAccountEmailRequest request);

    [Put("/api/accounts/status")]
    Task<IApiResponse> UpdateStatus([Body] UpdateAccountStatusRequest request);

    [Delete("/api/accounts/{id}")]
    Task<IApiResponse> Delete(int id);
}
