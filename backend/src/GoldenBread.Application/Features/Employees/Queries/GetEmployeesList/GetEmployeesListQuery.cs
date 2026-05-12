using GoldenBread.Application.Features.Employees.Dtos;

namespace GoldenBread.Application.Features.Employees.Queries.GetEmployeesList;

public sealed record GetEmployeesListQuery : IRequest<EmployeesListResponse>;
