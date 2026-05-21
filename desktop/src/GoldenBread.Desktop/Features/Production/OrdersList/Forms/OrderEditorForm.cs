using GoldenBread.Desktop.UI.Common;
using ReactiveUI;
using ReactiveUI.SourceGenerators;
using System.Collections.ObjectModel;

namespace GoldenBread.Desktop.Features.Production.OrdersList.Forms;

public partial class OrderEditorForm : ViewModelBase
{
    [Reactive]
    int _companyId;

    [Reactive]
    string? _companySearchText;

    [Reactive]
    DateOnly? _selectedEndDate;

    [Reactive]
    DateOnly? _minDeliveryDate;

    [Reactive]
    DateOnly? _maxDeliveryDate;

    public ObservableCollection<OrderCartItem> CartItems { get; } = new();
    public decimal TotalPrice => CartItems.Sum(x => x.TotalCost);
    public bool CanSelectDate => CartItems.Any();
    public bool CanCreate => CompanyId > 0 && CartItems.Any() && SelectedEndDate.HasValue;
}