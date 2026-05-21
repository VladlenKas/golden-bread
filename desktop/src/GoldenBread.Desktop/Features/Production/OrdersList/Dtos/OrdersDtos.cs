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
    OrderStatus Status);

public record UpdateOrderStatusRequest(int OrderId, OrderStatus NewStatus, string? CancelReason);

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
    List<EmployeeTaskDetail> Tasks,
    List<IngredientReservationDetail> Reservations);

public record OrderItemDetail(string ProductName, string BatchInfo, int Quantity, decimal TotalCost, OrderStatus Status);

public record EmployeeTaskDetail(int EmployeeTaskId, string EmployeeName, OrderStatus Status, DateTime? StartTime, DateTime? EndTime);

public record IngredientReservationDetail(
    string IngredientName,
    decimal ReservedQuantity,
    IngredientUnit Unit,
    DateOnly DeliveryDate,
    DateOnly ExpiryDate);

public record IngredientShortageItem(
    string IngredientName,
    decimal RequiredQuantity,
    decimal AvailableQuantity,
    IngredientUnit Unit);
