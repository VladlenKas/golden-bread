namespace GoldenBread.Application.Features.Employees.Dtos;

public record EmployeesListResponse(List<EmployeeListItem> EmployeesList);

public record EmployeeListItem(
    int EmployeeId,
    string Fullname,
    DateOnly Birthday,
    bool CanDelete);
