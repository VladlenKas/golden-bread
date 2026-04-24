namespace GoldenBread.Domain.Entities;

public class Supplier
{
    public int SupplierId { get; set; }

    public string Name { get; set; } = null!;
    public string? Email { get; set; }
    public string? Phone { get; set; }
    public string? Address { get; set; }
    public DateTime? DeletedAt { get; set; }

    public ICollection<SupplierIngredient> SupplierIngredients { get; set; } = new List<SupplierIngredient>();
}
