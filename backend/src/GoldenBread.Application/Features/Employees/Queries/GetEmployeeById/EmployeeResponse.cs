namespace GoldenBread.Application.Features.Employees.Queries.GetEmployeeById;

public record EmployeeResponse(
    int EmployeeId,
    string Firstname,
    string Lastname,
    string? Patronymic,
    DateTimeOffset Birthday);
