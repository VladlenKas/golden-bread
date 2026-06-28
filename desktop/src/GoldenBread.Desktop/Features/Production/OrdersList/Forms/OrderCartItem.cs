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

    [Reactive] public int _productionTimeMinutes;
    [Reactive] int _totalProductionTimeMinutes;

    public ObservableCollection<ProductBatchOption> AvailableBatches { get; } = new();

    public OrderCartItem()
    {
        this.WhenAnyValue(
                x => x.Quantity,
                x => x.SelectedBatch,
                x => x.ProductionTimeMinutes)
            .Subscribe(_ => Recalculate());
    }
    private void Recalculate()
    {
        var batch = SelectedBatch;
        if (batch == null || Quantity <= 0)
        {
            TotalUnits = 0;
            TotalCost = 0;
            TotalProductionTimeMinutes = 0; // ← ДОБАВИТЬ
            return;
        }

        TotalUnits = Quantity * batch.QuantityUnits;
        TotalCost = TotalUnits * batch.UnitPrice;

        TotalProductionTimeMinutes = Quantity * ProductionTimeMinutes;
    }
}