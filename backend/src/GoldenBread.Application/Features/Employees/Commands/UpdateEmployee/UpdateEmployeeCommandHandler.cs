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
        var employee = await employeeRepository.GetByIdAsync(request.EmployeeId, ct);

        if (employee is null)
            return false;

        employee.Update(
            request.Firstname, 
            request.Lastname, 
            request.Patronymic, 
            request.Birthday);

        await unitOfWork.SaveChangesAsync(ct);

        return true;
    }
}