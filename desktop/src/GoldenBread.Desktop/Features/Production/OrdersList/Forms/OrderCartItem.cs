using GoldenBread.Desktop.Features.Production.OrdersList.Models;
using GoldenBread.Desktop.UI.Common;
using ReactiveUI;
using ReactiveUI.SourceGenerators;
using System.Collections.ObjectModel;

namespace GoldenBread.Desktop.Features.Production.OrdersList.Forms;

public partial class OrderCartItem : ViewModelBase
{
    [Reactive] int _productId;
    [Reactive] string? _productName;
    [Reactive] decimal _costPrice;
    [Reactive] ProductBatchOption? _selectedBatch;
    [Reactive] int _quantity = 1;
    [Reactive] int _totalUnits;
    [Reactive] decimal _totalCost;

    public ObservableCollection<ProductBatchOption> AvailableBatches { get; } = new();

    public OrderCartItem()
    {
        this.WhenAnyValue(
                x => x.Quantity,
                x => x.SelectedBatch)
            .Subscribe(_ => Recalculate());
    }
    private void Recalculate()
    {
        var batch = SelectedBatch;
        if (batch == null || Quantity <= 0)
        {
            TotalUnits = 0;
            TotalCost = 0;
            return;
        }

        TotalUnits = Quantity * batch.QuantityUnits;
        TotalCost = TotalUnits * batch.UnitPrice;
    }
}