using GoldenBread.Application.Abstractions.Data;
using GoldenBread.Application.Features.CompanyProfile.Dtos;

namespace GoldenBread.Application.Features.Company.Queries.GetById;

public sealed class GetByIdQueryHandler(IGoldenBreadContext context) : 
    IRequestHandler<GetByIdQuery, ProfileResponse>
{
    public async Task<ProfileResponse> Handle(
        GetByIdQuery query, 
        CancellationToken cancellationToken)
    {
        // TODO: Добавить маппер Account -> CompanyInfoResponse

        //return await context.Accounts
        //    .FirstOrDefaultAsync(a => 
        //        a.AccountId == query.Id,
        //        cancellationToken) 
        //    ?? throw new AccountNotFoundException(query.Id);

        throw new NotImplementedException();
    }
}
