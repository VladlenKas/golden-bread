using DynamicData;
using GoldenBread.Desktop.Features.Statistics.Dtos;
using GoldenBread.Desktop.Features.Statistics.Models;
using GoldenBread.Desktop.Infrastructure.Api;
using GoldenBread.Desktop.UI.Common;
using GoldenBread.Desktop.UI.Services;
using LiveChartsCore;
using LiveChartsCore.SkiaSharpView;
using LiveChartsCore.SkiaSharpView.Painting;
using ReactiveUI;
using ReactiveUI.SourceGenerators;
using SkiaSharp;
using SukiUI.Controls;
using System.Collections.ObjectModel;
using System.Reactive.Linq;

namespace GoldenBread.Desktop.Features.Statistics.ViewModels;

public partial class StatisticsListPageViewModel : PageViewModel, ISukiStackPageTitleProvider
{
    private readonly IProductsApi _api;
    private readonly DialogService _dialogService;
    private readonly ToastService _toastService;
    private RawStatisticsData? _rawData;

    [Reactive] private bool _isBusy;
    [Reactive] private bool _isEmpty = true;
    [Reactive] private DateTime? _dateFrom;
    [Reactive] private DateTime? _dateTo;

    // KPI
    [Reactive] private decimal _totalRevenue;
    [Reactive] private int _totalUnitsSold;
    [Reactive] private int _totalOrders;
    [Reactive] private decimal _averageOrderValue;
    [Reactive] private double _overallCompletionRate;

    // Таблицы
    private readonly SourceList<ProductSalesDto> _productsSource = new();
    private readonly SourceList<CustomerMetricDto> _customersSource = new ();
    private readonly SourceList<ProductionEfficiencyDto> _productionSource = new ();

    public ReadOnlyObservableCollection<ProductSalesDto> TopProducts { get; }
    public ReadOnlyObservableCollection<CustomerMetricDto> TopCustomers { get; }
    public ReadOnlyObservableCollection<ProductionEfficiencyDto> ProductionEfficiency { get; }

    // Графики
    public ObservableCollection<ISeries> MonthlySeries { get; } = new();
    public ObservableCollection<Axis> MonthlyXAxes { get; } = new();
    public ObservableCollection<Axis> MonthlyYAxes { get; } = new();
    public ObservableCollection<ISeries> CategorySeries { get; } = new();
    public ObservableCollection<ISeries> SeasonalitySeries { get; } = new();
    public ObservableCollection<Axis> SeasonalityXAxes { get; } = new();

    public string Title { get; set; } = "Статистика продаж";

    public StatisticsListPageViewModel(
        IProductsApi api,
        DialogService dialogService,
        ToastService toastService)
    {
        _api = api;
        _dialogService = dialogService;
        _toastService = toastService;

        _productsSource.Connect().Bind(out var p).Subscribe();
        TopProducts = p;
        _customersSource.Connect().Bind(out var c).Subscribe();
        TopCustomers = c;
        _productionSource.Connect().Bind(out var pr).Subscribe();
        ProductionEfficiency = pr;

        this.WhenAnyValue(x => x.DateFrom, x => x.DateTo)
            .Throttle(TimeSpan.FromMilliseconds(300))
            .ObserveOn(RxApp.MainThreadScheduler)
            .Subscribe(_ => CalculateMetrics());
    }

    [ReactiveCommand]
    private async Task RefreshAsync()
    {
        try
        {
            IsBusy = true;
            var response = await _api.GetRawData(DateFrom, DateTo);

            if (!response.IsSuccessStatusCode || response.Content == null)
            {
                _toastService.ShowError("Ошибка загрузки аналитики");
                return;
            }

            _rawData = response.Content;
            CalculateMetrics();
        }
        catch (Exception ex)
        {
            _dialogService.ShowError($"Ошибка: {ex.Message}");
        }
        finally
        {
            IsBusy = false;
        }
    }

    [ReactiveCommand]
    private void ResetFilters()
    {
        DateFrom = null;
        DateTo = null;
    }

    private void CalculateMetrics()
    {
        if (_rawData == null) return;

        var orders = _rawData.Orders.AsEnumerable();

        if (DateFrom.HasValue)
            orders = orders.Where(o => o.CreatedAt >= DateFrom.Value);
        if (DateTo.HasValue)
            orders = orders.Where(o => o.CreatedAt <= DateTo.Value.AddDays(1).AddTicks(-1));

        var list = orders.ToList();

        if (!list.Any())
        {
            IsEmpty = true;
            ClearAll();
            return;
        }

        IsEmpty = false;

        // --- KPI ---
        TotalRevenue = list.Sum(o => o.Quantity * o.UnitsPerBatch * o.UnitPrice);
        TotalUnitsSold = list.Sum(o => o.Quantity * o.UnitsPerBatch);
        TotalOrders = list.Select(o => o.OrderId).Distinct().Count();
        AverageOrderValue = TotalOrders == 0 ? 0 : TotalRevenue / TotalOrders;

        // --- ВСЕ ПРОДУКТЫ ---
        var products = list
            .GroupBy(o => new { o.ProductId, o.ProductName, o.CategoryName, o.ProductionTimeMinutes })
            .Select(g => new ProductSalesDto
            {
                ProductId = g.Key.ProductId,
                ProductName = g.Key.ProductName,
                CategoryName = g.Key.CategoryName,
                TotalUnitsSold = g.Sum(o => o.Quantity * o.UnitsPerBatch),
                TotalRevenue = g.Sum(o => o.Quantity * o.UnitsPerBatch * o.UnitPrice),
                OrdersCount = g.Select(o => o.OrderId).Distinct().Count(),
                AverageUnitsPerOrder = g.Average(o => o.Quantity * o.UnitsPerBatch),
                ProductionTimeMinutes = g.Key.ProductionTimeMinutes,
                AverageMarkup = g.Average(o => (double)o.MarkupPercent)
            })
            .OrderByDescending(x => x.TotalUnitsSold)
            .ToList();

        _productsSource.Clear();
        _productsSource.AddRange(products);

        // --- СЕЗОННОСТЬ ---
        var seasonality = list
            .GroupBy(o => GetSeason(o.CreatedAt))
            .Select(g => new SeasonalityDto
            {
                Season = g.Key,
                TotalUnits = g.Sum(o => o.Quantity * o.UnitsPerBatch),
                TotalRevenue = g.Sum(o => o.Quantity * o.UnitsPerBatch * o.UnitPrice),
                OrdersCount = g.Select(o => o.OrderId).Distinct().Count()
            })
            .ToList();

        SeasonalitySeries.Clear();
        SeasonalitySeries.Add(new ColumnSeries<int>
        {
            Values = seasonality.Select(s => s.TotalUnits).ToList(),
            Name = "Штук",
            Fill = new SolidColorPaint(SKColors.Coral)
        });
        SeasonalityXAxes.Clear();
        SeasonalityXAxes.Add(new Axis
        {
            Labels = seasonality.Select(s => s.Season).ToList()
        });

        // --- ПОМЕСЯЧНАЯ ДИНАМИКА ---
        var monthly = list
            .GroupBy(o => new { o.CreatedAt.Year, o.CreatedAt.Month })
            .Select(g => new MonthlyTrendDto
            {
                Year = g.Key.Year,
                Month = g.Key.Month,
                Label = $"{g.Key.Month:D2}.{g.Key.Year}",
                TotalRevenue = g.Sum(o => o.Quantity * o.UnitsPerBatch * o.UnitPrice),
                TotalUnits = g.Sum(o => o.Quantity * o.UnitsPerBatch),
                OrdersCount = g.Select(o => o.OrderId).Distinct().Count()
            })
            .OrderBy(x => x.Year).ThenBy(x => x.Month)
            .ToList();

        MonthlySeries.Clear();
        MonthlySeries.Add(new LineSeries<decimal>
        {
            Values = monthly.Select(x => x.TotalRevenue).ToList(),
            Name = "Выручка",
            Stroke = new SolidColorPaint(SKColors.DodgerBlue, 3),
            Fill = null,
            GeometrySize = 8
        });
        MonthlyXAxes.Clear();
        MonthlyXAxes.Add(new Axis
        {
            Labels = monthly.Select(x => x.Label).ToList(),
            LabelsRotation = 45
        });
        MonthlyYAxes.Clear();
        MonthlyYAxes.Add(new Axis { Name = "₽" });

        // --- КАТЕГОРИИ (Pie) ---
        var categories = list
            .GroupBy(o => new { o.CategoryId, o.CategoryName, o.CategoryColor })
            .Select(g => new CategoryBreakdownDto
            {
                CategoryId = g.Key.CategoryId,
                CategoryName = g.Key.CategoryName,
                Color = g.Key.CategoryColor,
                TotalRevenue = g.Sum(o => o.Quantity * o.UnitsPerBatch * o.UnitPrice),
                TotalUnits = g.Sum(o => o.Quantity * o.UnitsPerBatch),
                OrdersCount = g.Select(o => o.OrderId).Distinct().Count(),
                ProductCount = g.Select(o => o.ProductId).Distinct().Count(),
                AverageMarkup = g.Average(o => (double)o.MarkupPercent)
            })
            .OrderByDescending(x => x.TotalRevenue)
            .ToList();

        CategorySeries.Clear();
        foreach (var cat in categories)
        {
            var skColor = SKColor.TryParse(cat.Color, out var c) ? c : SKColors.Gray;
            CategorySeries.Add(new PieSeries<decimal>
            {
                Values = new[] { cat.TotalRevenue },
                Name = cat.CategoryName,
                Fill = new SolidColorPaint(skColor),
                DataLabelsPaint = new SolidColorPaint(SKColors.White),
                DataLabelsSize = 12,
                DataLabelsPosition = LiveChartsCore.Measure.PolarLabelsPosition.Middle,
                InnerRadius = 50
            });
        }

        // --- ВСЕ КЛИЕНТЫ ---
        var customers = list
            .GroupBy(o => new { o.CompanyId, o.CompanyName })
            .Select(g => new CustomerMetricDto
            {
                CompanyId = g.Key.CompanyId,
                CompanyName = g.Key.CompanyName,
                TotalRevenue = g.Sum(o => o.Quantity * o.UnitsPerBatch * o.UnitPrice),
                TotalUnits = g.Sum(o => o.Quantity * o.UnitsPerBatch),
                OrdersCount = g.Select(o => o.OrderId).Distinct().Count(),
                AverageOrderValue = g.Select(o => o.OrderId).Distinct().Count() == 0
                    ? 0
                    : g.Sum(o => o.Quantity * o.UnitsPerBatch * o.UnitPrice) / g.Select(o => o.OrderId).Distinct().Count()
            })
            .OrderByDescending(x => x.TotalRevenue)
            .ToList();

        _customersSource.Clear();
        _customersSource.AddRange(customers);

        // --- ПРОИЗВОДСТВО ---
        if (_rawData.EmployeeTasks?.Any() == true)
        {
            var prod = _rawData.EmployeeTasks
                .GroupBy(t => new { t.ProductId, t.ProductName })
                .Select(g => new ProductionEfficiencyDto
                {
                    ProductId = g.Key.ProductId,
                    ProductName = g.Key.ProductName,
                    TotalTasks = g.Count(),
                    TotalAssigned = g.Sum(x => x.AssignedQuantity),
                    TotalCompleted = g.Sum(x => x.CompletedQuantity),
                    CompletionRate = g.Sum(x => x.AssignedQuantity) == 0
                        ? 0
                        : (double)g.Sum(x => x.CompletedQuantity) / g.Sum(x => x.AssignedQuantity) * 100,
                    AverageTaskDurationMinutes = g.Where(x => x.StartTime.HasValue && x.EndTime.HasValue)
                        .Average(x => (x.EndTime!.Value - x.StartTime!.Value).TotalMinutes)
                })
                .OrderByDescending(x => x.TotalTasks)
                .ToList();

            _productionSource.Clear();
            _productionSource.AddRange(prod);

            OverallCompletionRate = prod.Any() ? prod.Average(p => p.CompletionRate) : 0;
        }
        else
        {
            _productionSource.Clear();
            OverallCompletionRate = 0;
        }
    }

    private void ClearAll()
    {
        _productsSource.Clear();
        _customersSource.Clear();
        _productionSource.Clear();
        MonthlySeries.Clear();
        CategorySeries.Clear();
        SeasonalitySeries.Clear();
        TotalRevenue = TotalUnitsSold = TotalOrders = 0;
        AverageOrderValue = 0;
        OverallCompletionRate = 0;
    }

    private static string GetSeason(DateTime date) => date.Month switch
    {
        3 or 4 or 5 => "Весна",
        6 or 7 or 8 => "Лето",
        9 or 10 or 11 => "Осень",
        _ => "Зима"
    };
}