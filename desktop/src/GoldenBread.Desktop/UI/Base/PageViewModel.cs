using GoldenBread.Desktop.Configuration.Models;

namespace GoldenBread.Desktop.UI.Common;

public partial class PageViewModel : ViewModelBase
{
    public CrudPermissionConfig Permissions { get; set; } = new();
    public record ItemsAutoCompleteBox(int Id, string Name)
    {
        public override string ToString() => Name;
    };
}
