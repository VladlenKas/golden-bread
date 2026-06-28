namespace GoldenBread.Application.Features.EmployeeTasks.Dtos;

public record UpdateEmployeeTaskProgressRequest(int EmployeeTaskId, int CompletedQuantity);