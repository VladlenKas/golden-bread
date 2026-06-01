using GoldenBread.Desktop.Features.Common;

namespace GoldenBread.Desktop.Features.Production.OrdersList.Dtos;

public record OrderKanbanItem(
    int OrderId,
    string CompanyName,
    DateOnly? StartDate,
    DateOnly EndDate,
    DateTime? CreatedAt,
    decimal TotalAmount,
    int TotalOrderItems,
    int CompletedOrderItems,
    int TotalTasks,
    int CompletedTasks,
    OrderStatus Status);

public record UpdateOrderStatusRequest(int OrderId, OrderStatus NewStatus);

public record OrderEditorDataResponse(List<CompanyLookup> Companies, List<ProductEditorDto> Products);

public record CompanyLookup(int CompanyId, string Name);

public record ProductEditorDto(int ProductId, string Name, decimal CostPrice, List<ProductBatchEditorDto> Batches);

public record ProductBatchEditorDto(int ProductBatchId, int MarkupPercent, int QuantityUnits);

public record OrderItemDraft(int ProductBatchId, int Quantity);

public record CalculateDeliveryRequest(List<OrderItemDraft> Items, DateOnly DesiredEndDate);

public record CalculateDeliveryResponse(DateOnly MinDeliveryDate, DateOnly MaxDeliveryDate);

public record CreateOrderRequest(int CompanyId, List<OrderItemDraft> Items, DateOnly EndDate);

public record OrderDetailResponse(
    int OrderId,
    string CompanyName,
    OrderStatus Status,
    DateOnly? StartDate,
    DateOnly EndDate,
    decimal TotalAmount,
    DateTime CreatedAt,
    List<OrderItemDetail> Items,
    List<EmployeeTaskDetail> Tasks);

public record OrderItemDetail(
    int IdBatch,
    string ProductName,
    string BatchInfo, 
    int Quantity, 
    decimal TotalCost,
    int TotalTasks,
    int CompletedTasks);

public record EmployeeTaskDetail(
    int EmployeeTaskId,
    string EmployeeName,
    Features.Common.TaskStatus Status,
    DateTimeOffset? StartTime,
    DateTimeOffset? EndTime);