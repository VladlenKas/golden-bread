using GoldenBread.Desktop.Configuration.Models;
using ReactiveUI.SourceGenerators;

namespace GoldenBread.Desktop.UI.Common;

public partial class PageViewModel : ViewModelBase
{
    public CrudPermissionConfig Permissions { get; set; } = new();
}
