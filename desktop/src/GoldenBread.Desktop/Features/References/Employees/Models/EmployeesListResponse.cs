namespace GoldenBread.Desktop.Features.References.Employees.Models;

public record EmployeesListResponse(List<EmployeeListItem> EmployeesList);

public record EmployeeListItem(
    int EmployeeId,
    string Fullname,
    DateOnly Birthday,
    bool CanDelete)
{
    public string SearchText = $"{Fullname}{Birthday}{EmployeeId}".ToLowerInvariant();
};
