namespace GoldenBread.Desktop.Features.Production.OrdersList.Models;

public class KanbanItem
{
    public int Id { get; set; }
    public string Title { get; set; } = string.Empty;
    public string Description { get; set; } = string.Empty;
    public string Status { get; set; } = string.Empty;

    public int OrderId => Id;
    public string CompanyName { get; set; } = string.Empty;
    public DateOnly? StartDate { get; set; }
    public DateOnly EndDate { get; set; }
    public DateTime? CreatedAt { get; set; }
    public decimal TotalAmount { get; set; }
    public int TotalOrderItems { get; set; }
    public int TotalTasks { get; set; }        
    public int CompletedTasks { get; set; }

    public double Progress => TotalTasks == 0 ? 0 : (double)CompletedTasks / TotalTasks;
    public bool IsDraggable => Status is not ("Completed" or "Canceled");
    public bool IsEnabled => Status != "Canceled";

    public string SearchText => $"{OrderId}{CompanyName}{StartDate}{CreatedAt}{TotalAmount}{TotalOrderItems}".ToLowerInvariant();
    public string ProgressText => TotalTasks == 0
        ? "Ожидает распределения"
        : $"{CompletedTasks}/{TotalTasks} задач";
}