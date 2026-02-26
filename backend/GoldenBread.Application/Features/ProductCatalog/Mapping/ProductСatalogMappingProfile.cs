using GoldenBread.Application.Features.ProductCatalog.Dtos;

namespace GoldenBread.Application.Features.ProductCatalog.Mapping;

public class ProductСatalogMappingProfile : Profile
{
    public ProductСatalogMappingProfile()
    {
        // ProductsListResponse

        CreateMap<Entities.Product, ProductListItemResponse>()

            .ForMember(dest => dest.CategoryName,
                opt => opt.MapFrom(src => src.Category.Name))

            .ForMember(dest => dest.CategoryColor,
                opt => opt.MapFrom(src => src.Category.Color))

            .ForMember(dest => dest.QuantityPerBatch,
                opt => opt.MapFrom(src => src.ProductBatches
                    .OrderBy(pb => pb.QuantityPerBatch)
                    .Select(pb => pb.QuantityPerBatch)
                    .FirstOrDefault()))

            .ForMember(dest => dest.SalePrice,
                opt => opt.MapFrom(src => src.ProductBatches
                    .OrderBy(pb => pb.QuantityPerBatch)
                    .Select(s => s.SalePrice)
                    .FirstOrDefault()))

            .ForMember(dest => dest.ImageUrl,
                opt => opt.MapFrom(src => src.ProductImages
                    .Select(i => i.ImagePath)
                    .FirstOrDefault()))

            .ForMember(dest => dest.IsFavourite,
                opt => opt.MapFrom((src, dest, _, context) =>
                    src.Favourites.Any(f => 
                        f.CompanyId == (int)context.Items["CompanyId"])));

        // ProductDetailResponse


    }
}
