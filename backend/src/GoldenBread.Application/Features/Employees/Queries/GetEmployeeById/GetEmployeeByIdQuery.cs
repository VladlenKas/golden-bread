using GoldenBread.Application.Features.Employees.Dtos;

namespace GoldenBread.Application.Features.Employees.Queries.GetEmployeeById;

public sealed record GetEmployeeByIdQuery(int Id) : IRequest<EmployeeDto>;
