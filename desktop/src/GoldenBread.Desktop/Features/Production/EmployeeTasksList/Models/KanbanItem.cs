using ReactiveUI;
using ReactiveUI.SourceGenerators;

namespace GoldenBread.Desktop.Features.Production.EmployeeTasksList.Models;

public partial class KanbanItem : ReactiveObject
{
    public KanbanItem()
    {
        this.WhenAnyValue(
            x => x.CompletedQuantity,
            x => x.AssignedQuantity)
            .Subscribe(_ =>
            {
                Progress = AssignedQuantity == 0 ? 0 : (double)CompletedQuantity / AssignedQuantity;
            });
    }

    public int Id { get; set; }
    public string Title { get; set; } = string.Empty;
    public string Description { get; set; } = string.Empty;

    [Reactive] public string _status = string.Empty;

    public string EmployeeName { get; set; } = string.Empty;
    public string ProductName { get; set; } = string.Empty;
    public int OrderId { get; set; }
    public string CompanyName { get; set; } = string.Empty;
    public DateTimeOffset? StartTime { get; set; }
    public DateTimeOffset? EndTime { get; set; }

    [Reactive] int _assignedQuantity;

    [Reactive] public int _completedQuantity;

    public string SearchText =>
        $"{Title} {EmployeeName} {ProductName} {CompanyName} {OrderId}"
        .ToLowerInvariant();

    [Reactive] public double _progress;

    public bool IsEditable =>
        StartTime.HasValue && StartTime.Value.Date >= DateTimeOffset.Now.Date;

    public bool IsDraggable => Status != "Canceled";
}