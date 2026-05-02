namespace GoldenBread.Application.Features.Employees.Commands.DeleteEmployee;

public sealed record DeleteEmployeeCommand(int EmployeeId) : IRequest<bool>;

