namespace GoldenBread.Desktop.Features.References.Employees.Models;

public record class EmployeeDto(
    int EmployeeId,
    string Firstname,
    string Lastname,
    string? Patronymic,
    DateOnly Birthday);
