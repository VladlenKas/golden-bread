using GoldenBread.Application.Abstractions.Services;
using GoldenBread.Application.Features.CompanyProfile.Dtos;

namespace GoldenBread.Application.Features.CompanyProfile.Queries.GetProfile;

public sealed class GetProfileQueryHandler(
    ICurrentAccountContext accountContext,
    IMapper mapper) : 
    IRequestHandler<GetProfileQuery, ProfileResponse>
{
    public async Task<ProfileResponse> Handle(
        GetProfileQuery query,
        CancellationToken cancellationToken)
    {
        var account = await accountContext.GetAccountAsync(cancellationToken);
        return mapper.Map<ProfileResponse>(account.Company);
    }
}