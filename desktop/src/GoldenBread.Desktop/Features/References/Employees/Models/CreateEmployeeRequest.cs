namespace GoldenBread.Desktop.Features.References.Employees.Models;

public sealed record CreateEmployeeRequest(
    string Firstname,
    string Lastname,
    string? Patronymic,
    DateOnly Birthday);
