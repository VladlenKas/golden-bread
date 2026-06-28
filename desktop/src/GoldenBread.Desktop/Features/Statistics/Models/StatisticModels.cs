namespace GoldenBread.Desktop.Features.Statistics.Models;

public class ProductSalesDto
{
    public int ProductId { get; set; }
    public string ProductName { get; set; } = string.Empty;
    public string CategoryName { get; set; } = string.Empty;
    public int TotalUnitsSold { get; set; }
    public decimal TotalRevenue { get; set; }
    public int OrdersCount { get; set; }
    public double AverageUnitsPerOrder { get; set; }
    public int ProductionTimeMinutes { get; set; }
    public double AverageMarkup { get; set; }
}

public class SeasonalityDto
{
    public string Season { get; set; } = string.Empty;
    public int TotalUnits { get; set; }
    public decimal TotalRevenue { get; set; }
    public int OrdersCount { get; set; }
}

public class MonthlyTrendDto
{
    public int Year { get; set; }
    public int Month { get; set; }
    public string Label { get; set; } = string.Empty;
    public decimal TotalRevenue { get; set; }
    public int TotalUnits { get; set; }
    public int OrdersCount { get; set; }
}

public class CategoryBreakdownDto
{
    public int CategoryId { get; set; }
    public string CategoryName { get; set; } = string.Empty;
    public string Color { get; set; } = string.Empty;
    public decimal TotalRevenue { get; set; }
    public int TotalUnits { get; set; }
    public int OrdersCount { get; set; }
    public int ProductCount { get; set; }
    public double AverageMarkup { get; set; }
}

public class CustomerMetricDto
{
    public int CompanyId { get; set; }
    public string CompanyName { get; set; } = string.Empty;
    public decimal TotalRevenue { get; set; }
    public int TotalUnits { get; set; }
    public int OrdersCount { get; set; }
    public decimal AverageOrderValue { get; set; }
}

public class ProductionEfficiencyDto
{
    public int ProductId { get; set; }
    public string ProductName { get; set; } = string.Empty;
    public int TotalTasks { get; set; }
    public int TotalAssigned { get; set; }
    public int TotalCompleted { get; set; }
    public double CompletionRate { get; set; }
    public double? AverageTaskDurationMinutes { get; set; }
}