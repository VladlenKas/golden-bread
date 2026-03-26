using GoldenBread.Application.Features.CompanyProfile.Dtos;

namespace GoldenBread.Application.Features.CompanyProfile.Queries.GetProfile;

public sealed record GetProfileQuery : IRequest<ProfileResponse>;