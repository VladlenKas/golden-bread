using GoldenBread.Application.Abstractions.Data.Repositories;
using GoldenBread.Application.Features.Employees.Dtos;

namespace GoldenBread.Application.Features.Employees.Queries.GetEmployeeById;

public sealed class GetEmployeeByIdQueryHandler(IEmployeeRepository employeeRepository)
    : IRequestHandler<GetEmployeeByIdQuery, EmployeeDto?>
{
    public async Task<EmployeeDto?> Handle(
        GetEmployeeByIdQuery query,
        CancellationToken ct)
    {
        var employee = await employeeRepository.GetByIdAsync(query.Id, ct);

        if (employee == null)
            return null;

        return new EmployeeDto(
            employee.EmployeeId,
            employee.Firstname,
            employee.Lastname,
            employee.Patronymic,
            employee.Birthday);
    }
}
