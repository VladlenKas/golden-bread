using GoldenBread.Domain.Enums;

namespace GoldenBread.Application.Features.EmployeeTasks.Dtos;

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
    Domain.Enums.TaskStatus Status);

public record UpdateEmployeeTaskStatusRequest(int EmployeeTaskId, Domain.Enums.TaskStatus NewStatus);

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
    Domain.Enums.TaskStatus Status,
    decimal? TotalAmount,
    string BatchInfo);