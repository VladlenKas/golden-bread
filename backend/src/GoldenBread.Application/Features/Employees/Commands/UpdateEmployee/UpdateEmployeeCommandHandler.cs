using GoldenBread.Application.Abstractions.Data;
using GoldenBread.Application.Abstractions.Data.Repositories;

namespace GoldenBread.Application.Features.Employees.Commands.UpdateEmployee;

public sealed class UpdateEmployeeCommandHandler(
    IEmployeeRepository employeeRepository,
    IUnitOfWork unitOfWork)
    : IRequestHandler<UpdateEmployeeCommand, bool>
{
    public async Task<bool> Handle(UpdateEmployeeCommand request, CancellationToken ct)
    {
        var employeeRequest = request.EmployeeDto;
        var employee = await employeeRepository.GetByIdAsync(employeeRequest.EmployeeId, ct);

        if (employee is null)
            return false;

        employee.Update(
            employeeRequest.Firstname,
            employeeRequest.Lastname,
            employeeRequest.Patronymic,
            employeeRequest.Birthday);

        await unitOfWork.SaveChangesAsync(ct);

        return true;
    }
}