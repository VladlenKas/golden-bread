using GoldenBread.Application.Abstractions.Data;
using GoldenBread.Application.Abstractions.Data.Repositories;

namespace GoldenBread.Application.Features.Employees.Commands.DeleteEmployee;

public sealed class DeleteEmployeeCommandHandler(
    IEmployeeRepository employeeRepository,
    IUnitOfWork unitOfWork)
    : IRequestHandler<DeleteEmployeeCommand, bool>
{
    public async Task<bool> Handle(DeleteEmployeeCommand request, CancellationToken ct)
    {
        var employee = await employeeRepository.GetByIdAsync(request.EmployeeId, ct);

        if (employee is null)
            return false;

        employee.DeletedAt = DateTime.UtcNow;
        await unitOfWork.SaveChangesAsync(ct);

        return true;
    }
}