using GoldenBread.Application.Abstractions.Data.Repositories;

namespace GoldenBread.Application.Features.Employees.Queries.GetEmployeesList;

public sealed class GetEmployeesListQueryHandler(IEmployeeRepository employeeRepository)
    : IRequestHandler<GetEmployeesListQuery, EmployeesListResponse>
{
    public async Task<EmployeesListResponse> Handle(
        GetEmployeesListQuery query,
        CancellationToken ct)
    {
        var employeesList = await employeeRepository.GetAllAsync(ct);

        var employeesListRepository = employeesList.Select(employee =>
            new EmployeeListItem(
                employee.EmployeeId,
                employee.Fullname,
                employee.Birthday))
            .ToList();

        return new EmployeesListResponse(employeesListRepository);
    }
}
