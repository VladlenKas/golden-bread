namespace GoldenBread.Application.Features.Employees.Queries.GetEmployeesList;

public record EmployeesListResponse(List<EmployeeListItem> EmployeesList);

public record EmployeeListItem(
    int EmployeeId,
    string Fullname,
    DateOnly Birthday);
