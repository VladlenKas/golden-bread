using GoldenBread.Application.Abstractions.Data.Repositories;
using GoldenBread.Application.Features.Employees.Dtos;
using GoldenBread.Domain.Entities;

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
                employee.Birthday,
                CanDelete(employee)))
            .ToList();

        return new EmployeesListResponse(employeesListRepository);
    }

    private bool CanDelete(Employee employee)
    {
        return !employee.EmployeeTasks.Any(et =>
            et.Status == Domain.Enums.OrderStatus.Awaiting ||
            et.Status == Domain.Enums.OrderStatus.InProgress) ||
            employee.EmployeeTasks.Count == 0;
    }
}
