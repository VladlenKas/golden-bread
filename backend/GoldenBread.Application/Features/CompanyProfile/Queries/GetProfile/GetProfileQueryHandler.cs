using GoldenBread.Application.Abstractions.Services;
using GoldenBread.Application.Common.Exceptions.Domain;
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

        var company = account.Company ??
            throw new AccountHasNoCompanyException(account.AccountId);

        return new ProfileResponse(
            account.Email,
            company.Name,
            company.Inn,
            company.Ogrn,
            company.Phone,
            company.Address);
    }
}