using GoldenBread.Domain.Extensions;

namespace GoldenBread.Domain.Entities;

public sealed class Company
{
    public int CompanyId { get; private set; }

    public int? AccountId { get; private set; }

    public string Name { get; set; } = null!;
    public string Inn { get; set; } = null!;
    public string Ogrn { get; set; } = null!;
    public string? Phone { get; set; }
    public string? Address { get; set; }

    public ICollection<CartItem> CartItems { get; set; } = new List<CartItem>();
    public ICollection<Favorite> Favourites { get; set; } = new List<Favorite>();
    public ICollection<Order> Orders { get; set; } = new List<Order>();

    public Account Account { get; set; } = null!;

    private Company() { }

    public static Company Create(
        int companyId,
        int accountId,
        string name,
        string inn,
        string ogrn,
        string? phone = null,
        string? address = null)
    {
        return new Company
        {       
            CompanyId = companyId,
            AccountId = accountId,
            Name = name.ToUpperFirstChar()!,
            Inn = inn,
            Ogrn = ogrn,
            Phone = phone.StringOrNullNormalize(),
            Address = address.StringOrNullNormalize(),
        };
    }

    public void UpdateRequisites(
        string name, 
        string inn, 
        string ogrn)
    {
        Name = name.ToUpperFirstChar()!;
        Inn = inn;
        Ogrn = ogrn;
    }

    public void UpdateContacts(
        string? phone, 
        string? address)
    {
        Phone = phone.StringOrNullNormalize();
        Address = address.StringOrNullNormalize();
    }
}
