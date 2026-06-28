using GoldenBread.Desktop.Features.Common;

namespace GoldenBread.Desktop.Features.Production.EmployeeTasksList.Dtos;

public record EmployeeTaskKanbanItem(
    int EmployeeTaskId,
    string EmployeeName,
    string ProductName,
    int OrderId,
    string CompanyName,
    DateTimeOffset? StartTime,
    DateTimeOffset? EndTime,
    int AssignedQuantity,
    int CompletedQuantity,
    Features.Common.TaskStatus Status);

public record UpdateEmployeeTaskStatusRequest(int EmployeeTaskId, Features.Common.TaskStatus NewStatus);

public record EmployeeTaskDetailResponse(
    int EmployeeTaskId,
    string EmployeeName,
    string ProductName,
    int OrderId,
    string CompanyName,
    int AssignedQuantity,
    int CompletedQuantity,
    DateTimeOffset? StartTime,
    DateTimeOffset? EndTime,
    Features.Common.TaskStatus Status,
    decimal? TotalAmount,
    string BatchInfo);

public record UpdateEmployeeTaskProgressRequest(int EmployeeTaskId, int CompletedQuantity);