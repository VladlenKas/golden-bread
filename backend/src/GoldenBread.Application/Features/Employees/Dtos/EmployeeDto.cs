namespace GoldenBread.Application.Features.Employees.Dtos;

public record EmployeeDto(
    int EmployeeId,
    string Firstname,
    string Lastname,
    string? Patronymic,
    DateOnly Birthday);
