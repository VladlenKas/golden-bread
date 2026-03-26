using GoldenBread.Application.Features.CompanyCart.Dtos;
using GoldenBread.Domain.Entities;
using static GoldenBread.Application.Features.CompanyCart.Queries.GetCart.GetCartQueryHandler;

namespace GoldenBread.Application.Features.CompanyCart.Mapping;

public class CartMappingProfile : Profile
{
    public CartMappingProfile()
    {
        CreateMap<OrderTariff, TariffDto>();
    }
}

