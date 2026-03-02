using GoldenBread.Domain.Entities;

namespace GoldenBread.Domain.Extensions;

public static class ProductExtensions
{
    public static decimal GetTotalCostInCart(this Product product, int companyId)
    {
        if (companyId == 0) return 0m;

        return product.ProductBatches
            .SelectMany(pb => pb.CartItems)
            .Where(ci => ci.CompanyId == companyId)
            .Sum(ci => ci.TotalPrice);
    }

    public static int GetQuantityInCart(this Product product, int companyId)
    {
        if (companyId == 0) return 0;

        return product.ProductBatches
            .SelectMany(pb => pb.CartItems)
            .Where(ci => ci.CompanyId == companyId)
            .Sum(ci => ci.Quantity);
    }
}
