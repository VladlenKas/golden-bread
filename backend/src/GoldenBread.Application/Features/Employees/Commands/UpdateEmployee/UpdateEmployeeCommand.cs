namespace GoldenBread.Application.Features.Employees.Commands.UpdateEmployee;

public sealed record UpdateEmployeeCommand(
    int EmployeeId,
    string Firstname,
    string Lastname,
    string? Patronymic,
    DateOnly Birthday) : IRequest<bool>;
