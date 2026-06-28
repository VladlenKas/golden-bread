namespace GoldenBread.Desktop.Features.Statistics.Dtos;

/// <summary>
/// Все данные для статистики в одном объекте
/// </summary>
public class RawStatisticsData
{
    public List<OrderRawDto> Orders { get; set; } = new();
    public List<ProductRawDto> Products { get; set; } = new();
    public List<EmployeeTaskRawDto> EmployeeTasks { get; set; } = new();
    public List<FavoriteRawDto> Favorites { get; set; } = new();
}

/// <summary>
/// Заказ с позициями. Одна строка = один товар в заказе
/// </summary>
public class OrderRawDto
{
    public int OrderId { get; set; }
    public int CompanyId { get; set; }
    public string CompanyName { get; set; } = string.Empty;
    public DateTime CreatedAt { get; set; }
    public string Status { get; set; } = string.Empty;
    public int ProductId { get; set; }
    public string ProductName { get; set; } = string.Empty;
    public int CategoryId { get; set; }
    public string CategoryName { get; set; } = string.Empty;
    public string CategoryColor { get; set; } = string.Empty;
    public int Quantity { get; set; }
    public int UnitsPerBatch { get; set; }
    public decimal UnitPrice { get; set; }
    public int MarkupPercent { get; set; }
    public decimal CostPrice { get; set; }
    public int ProductionTimeMinutes { get; set; }
}

/// <summary>
/// Товар со справочной информацией
/// </summary>
public class ProductRawDto
{
    public int ProductId { get; set; }
    public string Name { get; set; } = string.Empty;
    public int CategoryId { get; set; }
    public string CategoryName { get; set; } = string.Empty;
    public string CategoryColor { get; set; } = string.Empty;
    public decimal CostPrice { get; set; }
    public int ProductionTimeMinutes { get; set; }
    public int ShelfLifeDays { get; set; }
}

/// <summary>
/// Задача сотрудника на производстве
/// </summary>
public class EmployeeTaskRawDto
{
    public int EmployeeTaskId { get; set; }
    public int ProductId { get; set; }
    public string ProductName { get; set; } = string.Empty;
    public int AssignedQuantity { get; set; }
    public int CompletedQuantity { get; set; }
    public DateTimeOffset? StartTime { get; set; }
    public DateTimeOffset? EndTime { get; set; }
    public string Status { get; set; } = string.Empty;
}

/// <summary>
/// Избранный товар (кто и что добавил в закладки)
/// </summary>
public class FavoriteRawDto
{
    public int ProductId { get; set; }
    public string ProductName { get; set; } = string.Empty;
    public int CompanyId { get; set; }
    public int CategoryId { get; set; }
}
