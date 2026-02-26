using System.ComponentModel.Design;

namespace GoldenBread.Domain.Entities;

public class Favourite
{
    public int FavouriteId { get; private set; }

    public int CompanyId { get; private set; }
    public int ProductId { get; private set; }

    public Product Product {    get; set; } = null!;
    public Company Company { get; set; } = null!;

    public Favourite() { }

    public static Favourite Create(
        int companyId,
        int productId)
    {   
        return new Favourite
        {
            ProductId = productId,
            CompanyId = companyId
        };
    }
}
        