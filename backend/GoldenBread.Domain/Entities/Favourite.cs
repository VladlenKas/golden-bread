namespace GoldenBread.Domain.Entities;

public sealed class Favourite
{
    public int FavouriteId { get; set; }
    public int CompanyId { get; set; }
    public int ProductId { get; set; }

    public Product Product { get; set; } = null!;
    public Company Company { get; set; } = null!;
}
