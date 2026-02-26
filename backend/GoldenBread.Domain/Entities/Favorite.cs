using System.ComponentModel.Design;

namespace GoldenBread.Domain.Entities;

public class Favorite
{
    public int FavoriteId { get; private set; }

    public int CompanyId { get; private set; }
    public int ProductId { get; private set; }

    public Product Product {    get; set; } = null!;
    public Company Company { get; set; } = null!;

    public Favorite() { }

    public static Favorite Create(
        int companyId,
        int productId)
    {   
        return new Favorite
        {
            ProductId = productId,
            CompanyId = companyId
        };
    }
}
        