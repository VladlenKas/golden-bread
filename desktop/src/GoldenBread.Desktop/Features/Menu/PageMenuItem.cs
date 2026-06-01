using Material.Icons;

namespace GoldenBread.Desktop.Features.Menu;

public sealed class PageMenuItem
{
    public string Key { get; init; } = string.Empty;
    public string Title { get; init; } = string.Empty;
    public MaterialIconKind Icon { get; set; }
    public int Order { get; init; }

    public override string ToString() => string.Empty;
}