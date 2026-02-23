using GoldenBread.Application.Features.CompanyProfile.Dtos;

namespace GoldenBread.Application.Features.Company.Queries.GetById;

public sealed record class GetByIdQuery(int Id) : 
    IRequest<ProfileResponse>;

