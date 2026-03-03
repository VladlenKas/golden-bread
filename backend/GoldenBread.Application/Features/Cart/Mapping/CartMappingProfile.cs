using GoldenBread.Application.Common.Mapping;
using GoldenBread.Application.Features.Cart.Dtos;

namespace GoldenBread.Application.Features.Cart.Mapping;

public class CartMappingProfile : Profile
{
    public CartMappingProfile()
    {
        // ProductCartItemResponse
        CreateMap<Entities.CartItem, ProductCartItemDto>()

            .ForMember(dest => dest.Name,
                opt => opt.MapFrom(src => src.Batch.Product.Name))

            .ForMember(dest => dest.ProductionTimeMinutes,
                opt => opt.MapFrom(src => src.Batch.Product.ProductionTimeMinutes))

            .ForMember(dest => dest.ProductBatchId,
                opt => opt.MapFrom(src => src.Batch.ProductBatchId))

            .ForMember(dest => dest.QuantityPerBatch,
                opt => opt.MapFrom(src => src.Batch.QuantityPerBatch))

            .ForMember(dest => dest.ImageUrl,
                opt => opt.MapFrom(src => src.Batch.Product.ProductImages
                    .Select(i => i.ImagePath)
                    .FirstOrDefault()))

            .ForMember(dest => dest.IsFavorite,
                opt => opt.MapFrom((src, _, _, ctx) =>
                    src.Batch.Product.Favourites.Any(f =>
                        f.CompanyId == ctx.GetCompanyId())))

            .ForMember(dest => dest.QuantityInCart,
                opt => opt.MapFrom(src => src.Quantity))

            .ForMember(dest => dest.TotalCostInCart,
                opt => opt.MapFrom(src =>
                    src.TotalCost));

        // TariffResponse
        CreateMap<Entities.OrderTariff, TariffDto>();
    }
}

