using GoldenBread.Application.Common.Mapping;
using GoldenBread.Application.Features.ProductCatalog.Dtos;
using GoldenBread.Domain.Extensions;

namespace GoldenBread.Application.Features.ProductCatalog.Mapping;

public class ProductCatalogMappingProfile : Profile
{
    public ProductCatalogMappingProfile()
    {
        // ProductListItemResponse
        CreateMap<Entities.Product, ProductListItemResponse>()

            .ForMember(dest => dest.CategoryName,
                opt => opt.MapFrom(src => src.Category.Name))

            .ForMember(dest => dest.CategoryColor,
                opt => opt.MapFrom(src => src.Category.Color))

            .ForMember(dest => dest.ProductBatchId,
                opt => opt.MapFrom((src, _, _, ctx) =>
                {
                    var cartBatch = src.ProductBatches
                        .FirstOrDefault(pb => pb.CartItems
                            .Any(ci => ci.CompanyId == ctx.GetCompanyId()));

                    return cartBatch?.ProductBatchId
                        ?? src.ProductBatches
                            .OrderBy(pb => pb.QuantityPerBatch)
                            .Select(pb => pb.ProductBatchId)
                            .FirstOrDefault();
                }))

            .ForMember(dest => dest.QuantityPerBatch,
                opt => opt.MapFrom((src, _, _, ctx) =>
                {
                    var cartBatch = src.ProductBatches
                        .FirstOrDefault(pb => pb.CartItems
                            .Any(ci => ci.CompanyId == ctx.GetCompanyId()));

                    return cartBatch?.QuantityPerBatch
                        ?? src.ProductBatches
                            .OrderBy(pb => pb.QuantityPerBatch)
                            .Select(pb => pb.QuantityPerBatch)
                            .FirstOrDefault();
                }))

            .ForMember(dest => dest.SalePrice,
                opt => opt.MapFrom((src, _, _, ctx) =>
                {
                    var cartBatch = src.ProductBatches
                        .FirstOrDefault(pb => pb.CartItems
                            .Any(ci => ci.CompanyId == ctx.GetCompanyId()));

                    return cartBatch?.UnitPrice
                        ?? src.ProductBatches
                            .OrderBy(pb => pb.QuantityPerBatch)
                            .Select(pb => pb.UnitPrice)
                            .FirstOrDefault();
                }))

            .ForMember(dest => dest.ImageUrl,
                opt => opt.MapFrom(src => src.ProductImages
                    .Select(i => i.ImagePath)
                    .FirstOrDefault()))

            .ForMember(dest => dest.IsFavorite,
                opt => opt.MapFrom((src, _, _, ctx) =>
                    src.Favourites.Any(f =>
                        f.CompanyId == ctx.GetCompanyId())))

            .ForMember(dest => dest.QuantityInCart,
                opt => opt.MapFrom((src, _, _, ctx) =>
                    src.ProductBatches
                        .SelectMany(pb => pb.CartItems)
                        .Where(ci => ci.CompanyId == ctx.GetCompanyId())
                        .Sum(ci => ci.Quantity)));

        // ProductDetailResponse
        CreateMap<Entities.Product, ProductDetailResponse>()

            .ForMember(dest => dest.CategoryName,
                opt => opt.MapFrom(src => src.Category.Name))

            .ForMember(dest => dest.CategoryColor,
                opt => opt.MapFrom(src => src.Category.Color))

            .ForMember(dest => dest.AvailableBatches,
                opt => opt.MapFrom(src => src.ProductBatches
                    .OrderBy(pb => pb.QuantityPerBatch)
                    .Select(pb => new ProductBatchResponse
                    {
                        ProductBatchId = pb.ProductBatchId,
                        QuantityPerBatch = pb.QuantityPerBatch,
                        UnitPrice = pb.UnitPrice,
                        TotalPrice = pb.TotalPrice,
                    })))

            .ForMember(dest => dest.ImageUrls,
                opt => opt.MapFrom(src => src.ProductImages
                    .Select(i => i.ImagePath)
                    .ToList()))

            .ForMember(dest => dest.IsFavorite,
                opt => opt.MapFrom((src, _, _, ctx) =>
                    src.Favourites.Any(f =>
                        f.CompanyId == ctx.GetCompanyId())))

            .ForMember(dest => dest.CurrentBatchId,
                opt => opt.MapFrom((src, _, _, ctx) =>
                    src.ProductBatches
                        .SelectMany(pb => pb.CartItems)
                        .FirstOrDefault(ci => ci.CompanyId == ctx.GetCompanyId())?
                        .BatchId ?? 0))

            .ForMember(dest => dest.Ingredients,
                opt => opt.MapFrom(src => src.Recipes
                    .Select(r => new IngredientResponse
                    {
                        IngredientId = r.Ingredient.IngredientId,
                        Name = r.Ingredient.Name,
                        Quantity = r.Quantity,
                        Unit = r.Ingredient.Unit.ToString(),
                    })))

            .ForMember(dest => dest.QuantityInCart,
                opt => opt.MapFrom((src, _, _, ctx) =>
                    src.GetQuantityInCart(ctx.GetCompanyId())))

            .ForMember(dest => dest.TotalCostInCart,
                opt => opt.MapFrom((src, _, _, ctx) =>
                    src.GetTotalCostInCart(ctx.GetCompanyId())));

        // ProductCategoryResponse
        CreateMap<Entities.ProductCategory, ProductCategoryResponse>()

            .ForMember(dest => dest.ProductCount,
                opt => opt.MapFrom(src => src.Products.Count()));
    }
}