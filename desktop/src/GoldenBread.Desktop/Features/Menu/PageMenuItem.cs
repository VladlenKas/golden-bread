namespace GoldenBread.Desktop.Features.Menu;

public sealed class PageMenuItem
{
    public string Key { get; init; } = string.Empty;
    public string Title { get; init; } = string.Empty;
    public int Order { get; init; }
}