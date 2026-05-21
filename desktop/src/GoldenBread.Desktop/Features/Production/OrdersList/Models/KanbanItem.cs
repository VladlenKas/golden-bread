using GoldenBread.Desktop.Features.Common;
using System.ComponentModel.Design;
using System.Xml.Linq;
using Tmds.DBus.Protocol;

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
    public int CompletedOrderItems { get; set; }
    public double Progress => TotalOrderItems == 0 ? 0 : (double)CompletedOrderItems / TotalOrderItems;
    public bool IsDraggable => Status is not ("Completed" or "Canceled");
    public bool IsEnabled => Status != "Canceled";

    public string SearchText => $"{OrderId}{CompanyName}{StartDate}{CreatedAt}{TotalAmount}{TotalOrderItems}{CompletedOrderItems}".ToLowerInvariant();

}