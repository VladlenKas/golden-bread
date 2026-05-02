using Material.Icons;

namespace GoldenBread.Desktop.Infrastructure.Helpers;

public static class ParseHelper
{
    public static MaterialIconKind ParseIcon(string icon) =>
        Enum.TryParse<MaterialIconKind>(icon, out var kind) ? kind : MaterialIconKind.Abacus;
}
