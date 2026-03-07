using GoldenBread.Application.Features.CompanyProfile.Dtos;

namespace GoldenBread.Application.Features.CompanyProfile.Mapping;

public class CompanyProfileMappingProfile : Profile
{
    public CompanyProfileMappingProfile()
    {
        CreateMap<Entities.Company, ProfileResponse>()

            .ForMember(dest => dest.Email, 
                opt => opt.MapFrom(src => src.Account!.Email));
    }
}
