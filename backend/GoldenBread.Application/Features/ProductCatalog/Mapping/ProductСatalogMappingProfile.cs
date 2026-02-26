using GoldenBread.Application.Features.ProductCatalog.Dtos;

namespace GoldenBread.Application.Features.ProductCatalog.Mapping;

public class ProductCatalogMappingProfile : Profile
{
    public ProductCatalogMappingProfile()
    {
        CreateMap<Entities.Product, ProductListItemResponse>()

            .ForMember(dest => dest.CategoryName,
                opt => opt.MapFrom(src => src.Category.Name))

            .ForMember(dest => dest.CategoryColor,
                opt => opt.MapFrom(src => src.Category.Color))

            .ForMember(dest => dest.ProductBatchId,
                opt => opt.MapFrom((src, dest, _, context) =>
                {
                    var companyId = (int)context.Items["CompanyId"];

                    var cartBatch = src.ProductBatches
                        .FirstOrDefault(pb => pb.CartItems
                            .Any(ci => ci.CompanyId == companyId));

                    return cartBatch?.ProductBatchId
                        ?? src.ProductBatches
                            .OrderBy(pb => pb.QuantityPerBatch)
                            .Select(pb => pb.ProductBatchId)
                            .FirstOrDefault();
                }))

            .ForMember(dest => dest.QuantityPerBatch,
                opt => opt.MapFrom((src, dest, _, context) =>
                {
                    var companyId = (int)context.Items["CompanyId"];

                    var cartBatch = src.ProductBatches
                        .FirstOrDefault(pb => pb.CartItems
                            .Any(ci => ci.CompanyId == companyId));

                    return cartBatch?.QuantityPerBatch
                        ?? src.ProductBatches
                            .OrderBy(pb => pb.QuantityPerBatch)
                            .Select(pb => pb.QuantityPerBatch)
                            .FirstOrDefault();
                }))

            .ForMember(dest => dest.SalePrice,
                opt => opt.MapFrom((src, dest, _, context) =>
                {
                    var companyId = (int)context.Items["CompanyId"];

                    var cartBatch = src.ProductBatches
                        .FirstOrDefault(pb => pb.CartItems
                            .Any(ci => ci.CompanyId == companyId));

                    return cartBatch?.SalePrice
                        ?? src.ProductBatches
                            .OrderBy(pb => pb.QuantityPerBatch)
                            .Select(pb => pb.SalePrice)
                            .FirstOrDefault();
                }))

            .ForMember(dest => dest.ImageUrl,
                opt => opt.MapFrom(src => src.ProductImages
                    .Select(i => i.ImagePath)
                    .FirstOrDefault()))

            .ForMember(dest => dest.IsFavorite,
                opt => opt.MapFrom((src, dest, _, context) =>
                    src.Favourites.Any(f =>
                        f.CompanyId == (int)context.Items["CompanyId"])))

            .ForMember(dest => dest.QuantityInCart,
                opt => opt.MapFrom((src, dest, _, context) =>
                {
                    var companyId = (int)context.Items["CompanyId"];

                    return src.ProductBatches
                        .SelectMany(pb => pb.CartItems)
                        .Where(ci => ci.CompanyId == companyId)
                        .Sum(ci => ci.Quantity);
                }));
    }
}