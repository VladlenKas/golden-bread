using GoldenBread.Application.Abstractions.Data;
using GoldenBread.Application.Features.EmployeeTasks.Dtos;
using Microsoft.EntityFrameworkCore;

namespace GoldenBread.Application.Features.EmployeeTasks.Commands;

public sealed record UpdateEmployeeTaskProgressCommand(
    UpdateEmployeeTaskProgressRequest Request) : IRequest<bool>;

public sealed class UpdateEmployeeTaskProgressCommandHandler(
    IGoldenBreadContext context,
    IUnitOfWork unitOfWork)
    : IRequestHandler<UpdateEmployeeTaskProgressCommand, bool>
{
    public async Task<bool> Handle(
        UpdateEmployeeTaskProgressCommand command,
        CancellationToken ct)
    {
        var req = command.Request;
        var task = await context.EmployeeTasks
            .AsTracking()
            .FirstOrDefaultAsync(t => t.EmployeeTaskId == req.EmployeeTaskId, ct);

        if (task is null) return false;

        // Редактирование только сегодняшних задач
        var taskDate = task.StartTime.HasValue
            ? DateOnly.FromDateTime(task.StartTime.Value.DateTime)
            : (DateOnly?)null;

        if (req.CompletedQuantity < 0 || req.CompletedQuantity > task.AssignedQuantity)
            throw new InvalidOperationException("Недопустимое количество выполненных единиц");

        await unitOfWork.BeginTransactionAsync(ct);
        try
        {
            task.UpdateCompletedQuantity(req.CompletedQuantity);
            await unitOfWork.CommitAsync(ct);
            return true;
        }
        catch
        {
            await unitOfWork.RollbackAsync(ct);
            throw;
        }
    }
}