namespace GoldenBread.Domain.Entities;

public class Company
{
    public int CompanyId { get; private set; }
    public int AccountId { get; private set; }
    public string Name { get; set; } = null!;
    public string Inn { get; set; } = null!;
    public string Ogrn { get; set; } = null!;
    public string? Phone { get; set; }
    public string? Address { get; set; }
    public Account Account { get; set; } = null!;

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
}
