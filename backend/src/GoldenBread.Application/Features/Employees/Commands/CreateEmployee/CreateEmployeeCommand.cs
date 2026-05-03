using GoldenBread.Application.Features.Employees.Dtos;

namespace GoldenBread.Application.Features.Employees.Commands.CreateEmployee;

public sealed record CreateEmployeeCommand(EmployeeDto EmployeeDto) : IRequest<int>;