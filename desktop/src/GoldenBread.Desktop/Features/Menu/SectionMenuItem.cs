using Material.Icons;

namespace GoldenBread.Desktop.Features.Menu;

public class SectionMenuItem
{
    public string Key { get; init; } = string.Empty;
    public string Title { get; init; } = string.Empty;
    public MaterialIconKind Icon { get; init; }
    public int Order { get; init; }

    public override string ToString() => string.Empty;
}