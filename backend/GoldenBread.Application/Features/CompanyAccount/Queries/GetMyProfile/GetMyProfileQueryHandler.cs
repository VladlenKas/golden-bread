using GoldenBread.Application.Features.CompanyAccount.Dtos;
using GoldenBread.Application.Services;

namespace GoldenBread.Application.Features.CompanyAccount.Queries.GetMyProfile;

public sealed class GetMyProfileQueryHandler(
    ICurrentAccountContext accountContext,
    IMapper mapper) : 
    IRequestHandler<GetMyProfileQuery, CompanyResponse>
{
    public async Task<CompanyResponse> Handle(
        GetMyProfileQuery query,
        CancellationToken cancellationToken)
    {
        var account = await accountContext.GetAccountAsync(cancellationToken);
        return mapper.Map<CompanyResponse>(account.Company);
    }
}