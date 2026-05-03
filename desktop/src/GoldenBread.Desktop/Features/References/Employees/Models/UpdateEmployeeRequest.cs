namespace GoldenBread.Desktop.Features.References.Employees.Models;

public sealed record UpdateEmployeeRequest(
    int EmployeeId,
    string Firstname,
    string Lastname,
    string? Patronymic,
    DateOnly Birthday);