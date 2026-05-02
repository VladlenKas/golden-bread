using GoldenBread.Application.Abstractions.Data;
using GoldenBread.Application.Abstractions.Data.Repositories;
using GoldenBread.Domain.Entities;
namespace GoldenBread.Application.Features.Employees.Commands.CreateEmployee;

public sealed class CreateEmployeeCommandHandler(
    IEmployeeRepository employeeRepository,
    IUnitOfWork unitOfWork)
    : IRequestHandler<CreateEmployeeCommand, int>
{
    public async Task<int> Handle(CreateEmployeeCommand request, CancellationToken ct)
    {
        var employee = Employee.Create(
            0,
            request.Firstname,
            request.Lastname,
            request.Patronymic,
            request.Birthday);

        await employeeRepository.AddAsync(employee, ct);
        await unitOfWork.SaveChangesAsync(ct);

        return employee.EmployeeId;
    }
}