using GoldenBread.Domain.Entities;

namespace GoldenBread.Domain.Extensions;

public static class ProductExtensions
{
    public static decimal GetTotalCostInCart(this Product product, int? companyId)
    {
        if (companyId is null or 0) return 0m;

        return GetCartItems(product, companyId).Sum(ci => ci.TotalCost);
    }

    public static int GetQuantityInCart(this Product product, int? companyId)
    {
        if (companyId is null or 0) return 0;

        return GetCartItems(product, companyId).Sum(ci => ci.Quantity);
    }

    private static IEnumerable<CartItem> GetCartItems(Product product, int? companyId)
    {
        return product.ProductBatches?
            .SelectMany(pb => pb.CartItems ?? Enumerable.Empty<CartItem>())
            .Where(ci => ci.CompanyId == companyId) ?? Enumerable.Empty<CartItem>();
    }
}