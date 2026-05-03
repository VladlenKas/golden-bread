using GoldenBread.Application.Features.Employees.Dtos;

namespace GoldenBread.Application.Features.Employees.Commands.UpdateEmployee;

public sealed record UpdateEmployeeCommand(EmployeeDto EmployeeDto) : IRequest<bool>;
