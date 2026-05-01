using GoldenBread.Application.Abstractions.Data.Repositories;

namespace GoldenBread.Application.Features.Employees.Queries.GetEmployeeById;

public sealed class GetEmployeeByIdQueryHandler(IEmployeeRepository employeeRepository)
    : IRequestHandler<GetEmployeeByIdQuery, EmployeeResponse?>
{
    public async Task<EmployeeResponse?> Handle(
        GetEmployeeByIdQuery query,
        CancellationToken ct)
    {
        var employee = await employeeRepository.GetByIdAsync(query.Id, ct);

        if (employee == null)
            return null;

        return new EmployeeResponse(
            employee.EmployeeId,
            employee.Firstname,
            employee.Lastname,
            employee.Patronymic,
            employee.Birthday.ToDateTime(new TimeOnly()));
    }
}
