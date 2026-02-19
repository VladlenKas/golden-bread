using GoldenBread.Application.Features.CompanyAccount.Dtos;

namespace GoldenBread.Application.Features.CompanyAccount.Mapping;

public class CompanyAccountMappingProfile : Profile
{
    public CompanyAccountMappingProfile()
    {
        CreateMap<Entities.Company, CompanyResponse>()
            .ForMember(dest => dest.Email, opt => opt.MapFrom(src => src.Account.Email));
    }
}
