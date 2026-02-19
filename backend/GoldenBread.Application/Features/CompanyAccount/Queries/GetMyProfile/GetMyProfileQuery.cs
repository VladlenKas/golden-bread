using GoldenBread.Application.Features.CompanyAccount.Dtos;

namespace GoldenBread.Application.Features.CompanyAccount.Queries.GetMyProfile;

public sealed record class GetMyProfileQuery :
    IRequest<CompanyResponse>;