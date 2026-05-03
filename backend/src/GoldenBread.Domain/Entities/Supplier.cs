using GoldenBread.Domain.Extensions;

namespace GoldenBread.Domain.Entities;

public class Supplier
{
    public int SupplierId { get; private set; }

    public string Name { get; set; } = null!;
    public string? Email { get; set; }
    public string? Phone { get; set; }
    public string? Address { get; set; }
    public DateTime? DeletedAt { get; set; }

    public ICollection<SupplierIngredient> SupplierIngredients { get; set; } = new List<SupplierIngredient>();

    public Supplier() { }

    public static Supplier Create(
        int supplierId,
        string name,
        string? email,
        string? phone,
        string? address,
        ICollection<SupplierIngredient>? supplierIngredients = null)
    {
        return new Supplier
        {
            SupplierId = supplierId,
            Name = name.ToUpperFirstChar()!,
            Email = email.StringOrNullNormalize(),
            Phone = phone.StringOrNullNormalize(),
            Address = address.StringOrNullNormalize(),
            SupplierIngredients = supplierIngredients ?? new List<SupplierIngredient>()
        };
    }

    public void Update(
        string name,
        string? email,
        string? phone,
        string? address)
    {
        Name = name.ToUpperFirstChar()!;
        Email = email.StringOrNullNormalize();
        Phone = phone.StringOrNullNormalize();
        Address = address.StringOrNullNormalize();
    }
}