using GoldenBread.Application.Abstractions.Services;
using GoldenBread.Application.Common.Constants;
using GoldenBread.Application.Common.Exceptions;
using GoldenBread.Application.Features.CompanyProfile.Dtos;

namespace GoldenBread.Application.Features.CompanyProfile.Queries.GetProfile;

public sealed class GetProfileQueryHandler(
    ICurrentAccountContext accountContext) : 
    IRequestHandler<GetProfileQuery, ProfileResponse>
{
    public async Task<ProfileResponse> Handle(
        GetProfileQuery query,
        CancellationToken ct)
    {
        var account = await accountContext.GetAccountAsync(ct);

        var company = account.GetCompany();

        return new ProfileResponse(
            account.Email,
            company.Name,
            company.Inn,
            company.Ogrn,
            company.Phone,
            company.Address);
    }
}