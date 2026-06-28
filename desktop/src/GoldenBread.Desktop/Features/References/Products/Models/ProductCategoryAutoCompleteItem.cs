namespace GoldenBread.Desktop.Features.References.Products.Models;

public record ProductCategoryAutoCompleteItem(int Id, string Name, string Color)
{
    public override string ToString() => Name;
}