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

    public Account? Account { get; set; }

    private Company() { }

    public static Company Create(
        string name,
        string inn,
        string ogrn,
        Account account,
        string? phone = null,
        string? address = null)
    {
        return new Company
        {       
            Name = name,
            Inn = inn,
            Ogrn = ogrn,
            Phone = phone,
            Address = address,
            Account = account
        };
    }

    public void UpdateRequisites(
        string name, 
        string inn, 
        string ogrn)
    {
        Name = name;
        Inn = inn;
        Ogrn = ogrn;
    }

    public void UpdateContacts(
        string? phone, 
        string? address)
    {
        Phone = phone;
        Address = address;
    }
}
