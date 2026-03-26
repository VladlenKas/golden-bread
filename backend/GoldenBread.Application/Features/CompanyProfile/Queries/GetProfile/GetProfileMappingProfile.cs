using GoldenBread.Application.Features.CompanyProfile.Dtos;
using GoldenBread.Domain.Entities;

namespace GoldenBread.Application.Features.CompanyProfile.Queries.GetProfile;

public class GetProfileMappingProfile : Profile
{
    public GetProfileMappingProfile()
    {
        CreateMap<Company, ProfileResponse>()

            .ForMember(dest => dest.Email, 
                opt => opt.MapFrom(src => src.Account.Email));
    }
}
