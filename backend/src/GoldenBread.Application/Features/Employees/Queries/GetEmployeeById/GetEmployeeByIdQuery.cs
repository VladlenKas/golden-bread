namespace GoldenBread.Application.Features.Employees.Queries.GetEmployeeById;

public sealed record GetEmployeeByIdQuery(int Id) : IRequest<EmployeeResponse>;
